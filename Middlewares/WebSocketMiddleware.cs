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
            PathString path = context.Request.Path;
            if (path.StartsWithSegments("/ws/"))
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    string endpoint = path.ToString().ToLower();
                    using WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    TaskCompletionSource<object> socketFinishedTcs = new TaskCompletionSource<object>();
                    await _webSocketManager.HandleWebSocketAsync(endpoint, webSocket, socketFinishedTcs);
                    await socketFinishedTcs.Task;
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

    public static class WebSocketMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketMiddleware>();
        }
    }
}
