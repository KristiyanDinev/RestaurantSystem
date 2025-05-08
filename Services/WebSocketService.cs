using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace RestaurantSystem.Services
{
    public class WebSocketService
    {
        // Dictionary of endpoint -> set of sockets
        private readonly ConcurrentDictionary<string, ConcurrentBag<WebSocket>> _endpointSockets = new();

        public async Task HandleWebSocketAsync(string endpoint, WebSocket socket)
        {
            var sockets = _endpointSockets.GetOrAdd(endpoint, _ => new ConcurrentBag<WebSocket>());
            sockets.Add(socket);

            var buffer = new byte[1024 * 4];
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Received on {endpoint}: {message}");
                    }
                }
            }
            finally
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                RemoveSocket(endpoint, socket);
            }
        }

        private void RemoveSocket(string endpoint, WebSocket socket)
        {
            if (_endpointSockets.TryGetValue(endpoint, out var sockets))
            {
                var newSockets = new ConcurrentBag<WebSocket>(sockets.Where(s => s != socket));
                _endpointSockets[endpoint] = newSockets;
            }
        }

        public async Task SendJsonAsync<T>(string endpoint, T data)
        {
            if (_endpointSockets.TryGetValue(endpoint, 
                out ConcurrentBag<WebSocket>? sockets))
            {
                string json = JsonSerializer.Serialize(data);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                ArraySegment<byte> segment = new ArraySegment<byte>(buffer);

                foreach (var socket in sockets.Where(s => s.State == WebSocketState.Open))
                {
                    try
                    {
                        await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch
                    {
                        // Optionally log and remove disconnected sockets
                    }
                }
            }
        }
    }
