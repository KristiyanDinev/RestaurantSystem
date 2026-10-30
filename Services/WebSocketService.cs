using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.WebSockets;
using RestaurantSystem.Utilities;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;

namespace RestaurantSystem.Services
{
    public class WebSocketService
    {
        // Dictionary of endpoint -> set of sockets
        private ConcurrentDictionary<string, ConcurrentBag<WebSocket>> _endpointSockets = new();
        private WebSocketUtility _webSocketUtility;
        private readonly bool distributionEnabled = false;

        private readonly ILogger<WebSocketService> _logger;
        private WebSocketDatabaseService _webSocketDatabaseService;
        private readonly DatabaseContext _databaseContext;
        private readonly UserManager<UserModel> _userManager;

        public WebSocketService(WebSocketUtility webSocketUtility,
            WebSocketDatabaseService webSocketDatabaseService,
            IOptions<RabbitMQOptionsModel> options, ILogger<WebSocketService> logger,
            DatabaseContext databaseContext, UserManager<UserModel> userManager)
        {
            RabbitMQOptionsModel model = options.Value;
            _logger = logger;
            _webSocketDatabaseService = webSocketDatabaseService;
            _webSocketUtility = webSocketUtility;
            _databaseContext = databaseContext;
            _userManager = userManager;

            if (model.Username == null ||
                model.Port < 1 ||
                model.Username == null ||
                model.Password == null) {
                return; // If options are not provided, skip initialization
            }
            distributionEnabled = true;
            
            // Initialize RabbitMQ connection
            _ = InitializeRabbitMQAsync(model);
        }

        /// Initialize RabbitMQ connection and set up message consumption
        private async Task InitializeRabbitMQAsync(RabbitMQOptionsModel options)
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

                _webSocketUtility.SetConnection(await factory.CreateConnectionAsync());
                IConnection connection = _webSocketUtility.GetConnection()!;
                _webSocketUtility.SetChannel(await connection.CreateChannelAsync());
                IChannel _channel = _webSocketUtility.GetChannel()!;

                string _exchangeName = _webSocketUtility.GetExchangeName();
                string _queueName = _webSocketUtility.GetQueueName();

                await _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Direct);

                await _channel.QueueDeclareAsync(queue: _queueName!, 
                    durable: false, 
                    exclusive: false, 
                    autoDelete: true);

                await _channel.QueueBindAsync(queue: _queueName!, 
                    exchange: _exchangeName, 
                    routingKey: _webSocketUtility.GetServerId());

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += OnMessageReceived;

                await _channel.BasicConsumeAsync(queue: _queueName, 
                    autoAck: true, consumer: consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to initialize RabbitMQ: {ex.Message}");
                _logger.LogWarning("WebSocket distribution is unavailable; local WebSockets remain enabled.");
            }
        }

        /// Handle incoming RabbitMQ messages for WebSocket distribution
        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs e)
        {
            try
            {

                WebSocketDistributionMessageModel? distributionMessage =
                    JsonSerializer.Deserialize<WebSocketDistributionMessageModel>(Encoding.UTF8.GetString(e.Body.ToArray()));
                if (distributionMessage == null) return;

                List<WebSocket> sockets = _webSocketUtility.GetListenersForOrderId(distributionMessage.OrderId);

                if (sockets.Count != 0)
                {
                    await SendJsonToClients(distributionMessage.Endpoint, distributionMessage.Data, sockets);
                }
            }
            catch
            {}
        }


        /// Handle JSON messages from WebSocket clients
        private async Task HandleJsonMessagesAsync(
            Dictionary<string, object> data, WebSocket socket, ClaimsPrincipal principal)
        {
            // The user sets Orders to listen to
            // list of order Ids
            if (data.TryGetValue("orders", out object? orders_obj))
            {
                List<string>? idsString = JsonSerializer.Deserialize<List<string>>(
                    orders_obj.ToString()!);
                if (idsString == null)
                {
                    return;
                }

                UserModel? user = await _userManager.GetUserAsync(principal);
                if (user == null)
                {
                    return;
                }
                IList<string> roles = await _userManager.GetRolesAsync(user);
                bool isStaff = roles.Any(role =>
                    role.Equals("cook", StringComparison.OrdinalIgnoreCase) ||
                    role.Equals("waitress", StringComparison.OrdinalIgnoreCase) ||
                    role.Equals("delivery", StringComparison.OrdinalIgnoreCase) ||
                    role.Equals("manager", StringComparison.OrdinalIgnoreCase));

                List<long> requestedIds = new();
                foreach (string id in idsString.Take(100))
                {
                    if (long.TryParse(id, out long orderId) && orderId > 0)
                    {
                        requestedIds.Add(orderId);
                    }
                }

                List<long> ids = await _databaseContext.Orders
                    .Where(order => requestedIds.Contains(order.Id) &&
                        (order.UserId == user.Id ||
                         (isStaff && user.RestaurantId.HasValue &&
                          order.RestaurantId == user.RestaurantId.Value)))
                    .Select(order => (long)order.Id)
                    .ToListAsync();
                foreach (string id in idsString)
                {
                    if (long.TryParse(id, out long orderId))
                    {
                        if (!ids.Contains(orderId))
                        {
                            continue;
                        }
                        if (!distributionEnabled)
                        {
                            continue;
                        }
                        await _webSocketDatabaseService
                            .AddOrderServerMappingAsync(orderId, _webSocketUtility.GetServerId());
                    }
                    else
                    {
                        continue;
                    }
                }

                _webSocketUtility.AddOrdersToListenTo(ids, socket);
            }
        }



        /// Handle WebSocket connection lifecycle
        public async Task HandleWebSocketAsync(string endpoint, WebSocket socket,
            ClaimsPrincipal principal,
            TaskCompletionSource<object> taskCompletionSource)
        {
            ConcurrentBag<WebSocket> sockets = _endpointSockets.GetOrAdd(endpoint, _ => new ConcurrentBag<WebSocket>());
            sockets.Add(socket);

            byte[] buffer = new byte[1024 * 4];
            try
            {
                while (socket.State.Equals(WebSocketState.Open))
                {
                    using MemoryStream messageBuffer = new();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await socket.ReceiveAsync(
                            new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            break;
                        }
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            messageBuffer.Write(buffer, 0, result.Count);
                            if (messageBuffer.Length > 64 * 1024)
                            {
                                await socket.CloseAsync(
                                    WebSocketCloseStatus.MessageTooBig,
                                    "Message too large", CancellationToken.None);
                                return;
                            }
                        }
                    } while (!result.EndOfMessage);

                    if (result.MessageType.Equals(WebSocketMessageType.Close))
                    {
                        break;
                    }
                    if (result.MessageType.Equals(WebSocketMessageType.Text))
                    {
                        Dictionary<string, object>? parsedMessage = JsonSerializer.Deserialize
                            <Dictionary<string, object>>(Encoding.UTF8.GetString(
                                messageBuffer.ToArray()));
                        if (parsedMessage == null)
                        {
                            continue;
                        }
                        await HandleJsonMessagesAsync(parsedMessage, socket, principal);
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
                await CleanupOrderMappingsAsync(socket);
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
        private async Task CleanupOrderMappingsAsync(WebSocket disconnectedSocket)
        {
            List<long> orderIds = _webSocketUtility.GetOrderIdsForSocket(disconnectedSocket);
            _webSocketUtility.RemoveOrderIdsFromSocket(disconnectedSocket);
            if (!distributionEnabled) { 
                return; // If distribution is not enabled, skip database cleanup
            }
            foreach (long orderId in orderIds)
            {
                await _webSocketDatabaseService.DeleteAllOrderServerMappingByOrderIdAsync(orderId);
            }
        }


        /// Send JSON message to a specific order across all servers
        /// This is the main method for distributed WebSocket messaging
        public async Task SendJsonToOrder<T>(string endpoint, long orderId, T data)
        {
            List<WebSocket> localSockets = _webSocketUtility.GetListenersForOrderId(orderId);
            if (localSockets.Count != 0)
            {
                await SendJsonToClients(endpoint, data, localSockets);
            }

            if (!distributionEnabled) { 
                return; 
            }
            OrderServerMappingModel? orderServerMapping =
                await _webSocketDatabaseService
                .GetServerWhichHasListenersForOrderIdAsync(orderId);

            string serverId = _webSocketUtility.GetServerId();
            if (orderServerMapping == null || orderServerMapping.ServerId.Equals(serverId))
            {
                return;
            }

            await PublishDistributionMessageAsync(new WebSocketDistributionMessageModel()
            {
                OrderId = orderId,
                Endpoint = endpoint,
                Data = data,
                TargetServerId = orderServerMapping.ServerId,
                SourceServerId = serverId,
                Timestamp = DateTime.UtcNow
            });
        }


        /// Publish a distribution message to RabbitMQ
        private async Task PublishDistributionMessageAsync(WebSocketDistributionMessageModel distributionMessage)
        {
            try
            {
                IChannel? channel = _webSocketUtility.GetChannel();
                if (channel == null)
                {
                    return;
                }
                await channel.BasicPublishAsync(
                    exchange: _webSocketUtility.GetExchangeName(),
                    routingKey: distributionMessage.TargetServerId,
                    body: new ReadOnlyMemory<byte>(
                        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(distributionMessage))));
            }
            catch {}
        }

        /// Send JSON message to specific WebSocket clients
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
                    await CleanupOrderMappingsAsync(socket);
                }
            }
        }
    }
}
