using ITStepFinalProject.Models;
using ITStepFinalProject.Utils.Controller;
using System.Net.WebSockets;

namespace ITStepFinalProject.Controllers
{
    public class WebSocketController
    {
        public static List<SubscribtionModel> orderSubscribtions = new List<SubscribtionModel>();
        public static List<SubscribtionModel> reservationSubscribtions = new List<SubscribtionModel>();
        public static List<SubscribtionModel> cookSubscribtions = new List<SubscribtionModel>();
        public WebSocketController(WebApplication app)
        {
            app.MapGet("/ws/orders", async (HttpContext context) =>
            {
                await HandleWholeRequest(context);
            }).RequireRateLimiting("fixed");

            app.MapGet("/ws/reservations", async (HttpContext context) =>
            {
                await HandleWholeRequest(context);
            }).RequireRateLimiting("fixed");

            app.MapGet("/ws/cook", async (HttpContext context) =>
            {
                await HandleWholeRequest(context);
            }).RequireRateLimiting("fixed");
        }

        private static async Task HandleWholeRequest(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                await HandleWebSocket(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private static async Task HandleWebSocket(HttpContext context)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

            var socketFinishedTcs = new TaskCompletionSource<object>();
            string path = context.Request.Path.Value ?? "";
            SubscribtionModel subscribtionModel = new SubscribtionModel(path, 
                webSocket,
            socketFinishedTcs);

            Console.WriteLine("\nNew WebSocket Connection.");

            subscribtionModel.WhileOpen(async (WebSocket socket) =>
            {
                string text = await subscribtionModel.ReceiveTextFromClient() ?? "";
                if (text.Length == 0)
                {
                    return;
                }

                List<string> parts = text.Split(';').ToList();
                if (parts.Count == 0)
                {
                    return;
                }
                Console.WriteLine("\nWebSocket Message from Client: " + text);

                string title = parts[0];

                switch (title)
                {
                    // subscribtion_ids;1;2;3;4  | set the ids of the subscribtion
                    case "subscribtion_ids":
                        {
                            subscribtionModel.HandleUpdateModelIds(parts);
                            break;
                        }
                    case "cook_status":
                        {
                            WebSocketUtils.HandleCookStatus(parts);
                            break;
                        }
                }
            });

            if (path.EndsWith("/cook"))
            {
                cookSubscribtions.Add(subscribtionModel);

            } else if (path.EndsWith("/reservations"))
            {
                reservationSubscribtions.Add(subscribtionModel);

            } else if (path.EndsWith("/orders"))
            {
                orderSubscribtions.Add(subscribtionModel);
            }
                
            await socketFinishedTcs.Task;
        }
    }
}
