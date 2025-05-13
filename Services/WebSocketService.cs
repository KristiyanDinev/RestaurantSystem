using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace RestaurantSystem.Services
{
    public class WebSocketService
    {
        // Dictionary of endpoint -> set of sockets
        private ConcurrentDictionary<string, ConcurrentBag<WebSocket>> _endpointSockets = new();
        private OrderService _orderService;

        public WebSocketService(OrderService orderService)
        {
            _orderService = orderService;
        }

        private void HandleJsonMessages(Dictionary<string, object> data, WebSocket socket)
        {
            // The user sets Orders to listen to
            if (data.TryGetValue("orders", out object? orders_obj))
            {

                // list of order Ids
                _orderService.AddOrdersToListenTo((List<int>)orders_obj, socket);
            }
        }


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
                        if (message == null) {
                            break;
                        }

                        HandleJsonMessages(message, socket);
                    }
                }
            }
            finally
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                RemoveSocket(endpoint, socket);
                taskCompletionSource.TrySetResult(endpoint);
            }
        }

        private void RemoveSocket(string endpoint, WebSocket socket)
        {
            if (_endpointSockets.TryGetValue(endpoint, out ConcurrentBag<WebSocket>? sockets))
            {
                _endpointSockets[endpoint] = new ConcurrentBag<WebSocket>(sockets.Where(s => !s.Equals(socket)));
            }
        }


        public async Task SendJsonToAll<T>(string endpoint, T data)
        {
            if (_endpointSockets.TryGetValue(endpoint,
                out ConcurrentBag<WebSocket>? sockets))
            {
                ArraySegment<byte> segment = new ArraySegment<byte>(
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data)));

                await SendJsonToClients<T>(endpoint, data,
                    sockets.Where(s => s.State.Equals(WebSocketState.Open)).ToList());
            }
        }


        public async Task SendJsonToClients<T>(string endpoint, T data, List<WebSocket> sockets)
        {
            foreach (WebSocket socket in sockets)
            {
                if (!socket.State.Equals(WebSocketState.Open))
                {
                    return;
                }

                string json = JsonSerializer.Serialize(data);
                ArraySegment<byte> segment = new ArraySegment<byte>(
                        Encoding.UTF8.GetBytes(json));

                try
                {
                    Console.WriteLine($"WebSocket: Sending to {endpoint}: {json}");
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch
                {
                    RemoveSocket(endpoint, socket);
                }
            }
        }
    }
}
