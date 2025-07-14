using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.WebSockets;
using RestaurantSystem.Utilities;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace RestaurantSystem.Services
{
    public class WebSocketService
    {
        // Dictionary of endpoint -> set of sockets
        private ConcurrentDictionary<string, ConcurrentBag<WebSocket>> _endpointSockets = new();
        private WebSocketUtility _webSocketUtility;

        // RabbitMQ components
        private IConnection? _connection = null;
        private IChannel? _channel = null;
        public readonly string? _serverId = null;
        private readonly string _exchangeName = "websocket_distribution";
        private readonly string? _queueName = null;

        private readonly bool distributionEnabled = false;

        private readonly ILogger<WebSocketService> _logger;
        private WebSocketDatabaseService _webSocketDatabaseService;

        public WebSocketService(WebSocketUtility webSocketUtility,
            WebSocketDatabaseService webSocketDatabaseService,
            IOptions<RabbitMQOptionsModel> options, ILogger<WebSocketService> logger)
        {
            RabbitMQOptionsModel model = options.Value;
            _logger = logger;
            _webSocketDatabaseService = webSocketDatabaseService;
            _webSocketUtility = webSocketUtility;

            if (model.Username == null ||
                model.Port < 1 ||
                model.Username == null ||
                model.Password == null) {
                return; // If options are not provided, skip initialization
            }
            distributionEnabled = true; // Enable distribution if options are provided
            // Generate unique server ID for this instance
            _serverId = Environment.MachineName + "_" + Guid.NewGuid().ToString("N")[..8];
            _queueName = $"websocket_queue_{_serverId}";
            
            // Initialize RabbitMQ connection
            InitializeRabbitMQ(model);
        }

        /// <summary>
        /// Initialize RabbitMQ connection and set up message consumption
        /// </summary>
        private async void InitializeRabbitMQ(RabbitMQOptionsModel options)
        {
            try
            {
                // Create connection factory
                var factory = new ConnectionFactory()
                {
                    HostName = options.HostName,
                    Port = options.Port,
                    UserName = options.Username,
                    Password = options.Password
                };

                // Create connection and channel
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                // Declare exchange for WebSocket message distribution
                await _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Direct);

                // Declare queue for this server instance
                await _channel.QueueDeclareAsync(queue: _queueName!, durable: false, exclusive: false, autoDelete: true);

                // Bind queue to exchange with server ID as routing key
                await _channel.QueueBindAsync(queue: _queueName!, exchange: _exchangeName, routingKey: _serverId!);

                // Set up message consumer
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += OnMessageReceived;

                // Start consuming messages
                await _channel.BasicConsumeAsync(queue: _queueName!, autoAck: true, consumer: consumer);

                _logger.LogInformation($"WebSocket Service initialized with Server ID: {_serverId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to initialize RabbitMQ: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Handle incoming RabbitMQ messages for WebSocket distribution
        /// </summary>
        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs e)
        {
            try
            {

                // Deserialize the distribution message
                WebSocketDistributionMessageModel? distributionMessage =
                    JsonSerializer.Deserialize<WebSocketDistributionMessageModel>(Encoding.UTF8.GetString(e.Body.ToArray()));
                if (distributionMessage == null) return;

                _logger.LogInformation($"Received distribution message for Order ID: {distributionMessage.OrderId}");

                // Get local WebSocket connections listening to this order
                List<WebSocket> sockets = _webSocketUtility.GetListenersForOrderId(distributionMessage.OrderId);

                // Send message to local connections
                if (sockets.Any())
                {
                    await SendJsonToClients(distributionMessage.Endpoint, distributionMessage.Data, sockets);
                    _logger.LogInformation($"Delivered message to {sockets.Count} local connections for Order ID: {distributionMessage.OrderId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing RabbitMQ message: {ex.Message}");
            }
        }


        /// <summary>
        /// Handle JSON messages from WebSocket clients
        /// </summary>
        private async void HandleJsonMessages(Dictionary<string, object> data, WebSocket socket)
        {
            // The user sets Orders to listen to
            // list of order Ids
            if (data.TryGetValue("orders", out object? orders_obj))
            {
                List<string>? idsString = JsonSerializer.Deserialize<List<string>>(orders_obj.ToString()!);
                if (idsString == null)
                {
                    return;
                }

                List<long> ids = new List<long>();
                foreach (string id in idsString)
                {
                    if (long.TryParse(id, out long orderId))
                    {
                        ids.Add(orderId);
                        // Register this server as handling this order ID
                        if (!distributionEnabled)
                        {
                            continue; // Distribution is not enabled, skip database mapping
                        }
                        await _webSocketDatabaseService.AddOrderServerMappingAsync(orderId, _serverId!);
                    }
                    else
                    {
                        continue; // Invalid order ID format
                    }
                }

                _webSocketUtility.AddOrdersToListenTo(ids, socket);
                _logger.LogInformation($"Client registered to listen to {ids.Count} orders on server {_serverId}");
            }
        }



        /// <summary>
        /// Handle WebSocket connection lifecycle
        /// </summary>
        public async Task HandleWebSocketAsync(string endpoint, WebSocket socket,
            TaskCompletionSource<object> taskCompletionSource)
        {
            ConcurrentBag<WebSocket> sockets = _endpointSockets.GetOrAdd(endpoint, _ => new ConcurrentBag<WebSocket>());
            sockets.Add(socket);

            byte[] buffer = new byte[1024 * 4];
            try
            {
                while (socket.State.Equals(WebSocketState.Open))
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType.Equals(WebSocketMessageType.Close))
                    {
                        break;
                    }
                    if (result.MessageType.Equals(WebSocketMessageType.Text))
                    {
                        // expect a JSON text
                        Dictionary<string, object>? message = JsonSerializer.Deserialize
                            <Dictionary<string, object>>(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        if (message == null)
                        {
                            break;
                        }
                        HandleJsonMessages(message, socket);
                    }
                }
            }
            finally
            {
                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                }
                catch { }
                RemoveSocket(endpoint, socket);
                CleanupOrderMappings(socket);
                taskCompletionSource.TrySetResult(endpoint);
            }
        }


        /// <summary>
        /// Remove socket from endpoint collections
        /// </summary>
        private void RemoveSocket(string endpoint, WebSocket socket)
        {
            if (_endpointSockets.TryGetValue(endpoint, out ConcurrentBag<WebSocket>? sockets))
            {
                _endpointSockets[endpoint] = new ConcurrentBag<WebSocket>(sockets.Where(s => !s.Equals(socket)));
            }
        }


        /// <summary>
        /// Clean up order mappings when a socket disconnects
        /// </summary>
        private async void CleanupOrderMappings(WebSocket disconnectedSocket)
        {
            // Get all order IDs that this socket was listening to
            List<long> orderIds = _webSocketUtility.GetOrderIdsForSocket(disconnectedSocket);
            _webSocketUtility.RemoveOrderIdsFromSocket(disconnectedSocket);
            _logger.LogInformation($"Cleaned up mappings for disconnected socket (local)");
            if (!distributionEnabled) { 
                return; // If distribution is not enabled, skip database cleanup
            }
            foreach (long orderId in orderIds)
            {
                await _webSocketDatabaseService.DeleteAllOrderServerMappingByOrderIdAsync(orderId);
            }
            _logger.LogInformation($"Cleaned up mappings for disconnected socket (distributed)");
        }


        /// <summary>
        /// Send JSON message to a specific order across all servers
        /// This is the main method for distributed WebSocket messaging
        /// </summary>
        public async Task SendJsonToOrder<T>(string endpoint, long orderId, T data)
        {
            // First, try to deliver locally
            List<WebSocket> localSockets = _webSocketUtility.GetListenersForOrderId(orderId);
            if (localSockets.Any())
            {
                await SendJsonToClients(endpoint, data, localSockets);
                _logger.LogInformation($"Delivered message locally to {localSockets.Count} connections for Order ID: {orderId}");
            }

            if (!distributionEnabled) { 
                return; // If distribution is not enabled, skip further processing
            }
            // Then, check if we need to send to other servers
            OrderServerMappingModel? orderServerMapping =
                await _webSocketDatabaseService
                .GetServerWhichHasListenersForOrderIdAsync(orderId);

            if (orderServerMapping == null || orderServerMapping.ServerId.Equals(_serverId))
            {
                return; // No listeners found, nothing to do
            }

            await PublishDistributionMessageAsync(new WebSocketDistributionMessageModel()
            {
                OrderId = orderId,
                Endpoint = endpoint,
                Data = data,
                TargetServerId = orderServerMapping.ServerId,
                SourceServerId = _serverId!,
                Timestamp = DateTime.UtcNow
            });
            _logger.LogInformation($"Sent distribution message to server {orderServerMapping.ServerId} for Order ID: {orderId}");
        }


        /// <summary>
        /// Publish a distribution message to RabbitMQ
        /// </summary>
        private async Task PublishDistributionMessageAsync(WebSocketDistributionMessageModel distributionMessage)
        {
            try
            {
                // Publish to exchange with target server ID as routing key
                await _channel!.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: distributionMessage.TargetServerId,
                    body: new ReadOnlyMemory<byte>(
                        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(distributionMessage))));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to publish distribution message: {ex.Message}");
            }
        }

        /// <summary>
        /// Send JSON message to specific WebSocket clients
        /// </summary>
        public async Task SendJsonToClients<T>(string endpoint, T data, List<WebSocket> sockets)
        {
            foreach (WebSocket socket in sockets)
            {
                if (!socket.State.Equals(WebSocketState.Open))
                {
                    continue;
                }

                string json = JsonSerializer.Serialize(data);
                ArraySegment<byte> segment = new ArraySegment<byte>(
                        Encoding.UTF8.GetBytes(json));

                try
                {
                    _logger.LogInformation($"WebSocket: Sending to {endpoint}: {json}");
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch
                {
                    try
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
                    }
                    catch { }
                    RemoveSocket(endpoint, socket);
                    CleanupOrderMappings(socket);
                }
            }
        }
    }
}
