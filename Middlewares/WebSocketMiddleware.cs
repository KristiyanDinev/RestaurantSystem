using RestaurantSystem.Services;
using System.Net.WebSockets;

namespace RestaurantSystem.Middlewares
{
    public class WebSocketMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly WebSocketService _webSocketManager;

        public WebSocketMiddleware(RequestDelegate next, WebSocketService manager)
        {
            _next = next;
            _webSocketManager = manager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/ws"))
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    string endpoint = context.Request.Path.ToString().ToLower();
                    using WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await _webSocketManager.HandleWebSocketAsync(endpoint, webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
