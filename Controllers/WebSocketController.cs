using ITStepFinalProject.Models;
using System.Net.WebSockets;

namespace ITStepFinalProject.Controllers
{
    public class WebSocketController
    {
        public static List<SubscribtionModel> orderSubscribtions = new List<SubscribtionModel>();
        public static List<SubscribtionModel> reservationSubscribtions = new List<SubscribtionModel>();
        public WebSocketController(WebApplication app)
        {
            app.MapGet("/ws/orders", async (HttpContext context) =>
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

            SubscribtionModel subscribtionModel = new SubscribtionModel("/orders", webSocket,
                socketFinishedTcs);

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

                string title = parts[0];
                parts.RemoveAt(0);

                switch (title)
                {
                    // subscribtion_ids;1;2;3;4  | set the ids of the subscribtion
                    case "subscribtion_ids":
                        {
                            subscribtionModel.HandleUpdateModelIds(parts);
                            break;
                        }
                }
            });

            orderSubscribtions.Add(subscribtionModel);

            await socketFinishedTcs.Task;
        }
    }
}
