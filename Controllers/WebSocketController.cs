using ITStepFinalProject.Models;
using System.Net.WebSockets;
using System.Text;

namespace ITStepFinalProject.Controllers
{
    public class WebSocketController
    {
        public List<SubscribtionModel> subscribtions;

        public WebSocketController(WebApplication app)
        {
            subscribtions = new List<SubscribtionModel>();

            app.MapGet("/ws/orders", async (HttpContext context) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    var socketFinishedTcs = new TaskCompletionSource<object>();

                    SubscribtionModel subscribtionModel = new SubscribtionModel("/orders", webSocket,
                        [], socketFinishedTcs);

                    subscribtionModel.WhileOpen(async (WebSocket socket) =>
                    {
                        string text = await subscribtionModel.ReceiveTextFromClient() ?? "";
                        if (text.Length == 0)
                        {
                            return;
                        }

                        // subscribtion_ids=1;2;3;4  | set the ids of the subscribtion
                        if (text.StartsWith("subscribtion_ids="))
                        {
                            try
                            {
                                List<int> Ids = new List<int>();
                                foreach (string IdStr in text.Split('=')[1].Split(';'))
                                {
                                    if (int.TryParse(IdStr, out int Id))
                                    {
                                        Ids.Add(Id);
                                    }
                                }
                                subscribtionModel.Ids = Ids;

                            }
                            catch (Exception) { }
                        }
                    });

                    subscribtions.Add(subscribtionModel);

                    await socketFinishedTcs.Task;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }).RequireRateLimiting("fixed");
        }
    }
}
