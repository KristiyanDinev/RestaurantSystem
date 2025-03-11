using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using System.Net.WebSockets;

namespace ITStepFinalProject.Controllers.WebSocketHandlers
{
    public class WebSocketHandler
    {
        public static List<SubscribtionModel> orderSubscribtions = new List<SubscribtionModel>();
        public static List<SubscribtionModel> reservationSubscribtions = new List<SubscribtionModel>();
        public static List<SubscribtionModel> cookSubscribtions = new List<SubscribtionModel>();

        private void RemoveModelIdFromSubscribtion(int id,
            List<SubscribtionModel> subscribtionModels)
        {
            foreach (SubscribtionModel subscribtionModel in subscribtionModels)
            {
                subscribtionModel.ModelIds.Remove(id);
            }
        }

        private static void CloseAllSubscribtions(List<SubscribtionModel> subscribtionModels)
        {
            foreach (SubscribtionModel subscribtionModel in subscribtionModels)
            {
                subscribtionModel.CloseWebSocket();
            }
        }

        public void RemoveModelIdFromOrderSubscribtion(int id)
        {
            RemoveModelIdFromSubscribtion(id, orderSubscribtions);
        }

        public void RemoveModelIdFromReservationSubscribtion(int id)
        {
            RemoveModelIdFromSubscribtion(id, reservationSubscribtions);
        }

        public static void CloseAllSubscribtions()
        {
            CloseAllSubscribtions(orderSubscribtions);
            CloseAllSubscribtions(reservationSubscribtions);
            CloseAllSubscribtions(cookSubscribtions);
        }

        public async static void HandleCookStatus(List<string> parts, 
            OrderDatabaseHandler orderDatabaseHandler)
        {
            // 1  or  di s
            if (parts.Count != 4 || !int.TryParse(parts[1], out int orderId) ||
                                !int.TryParse(parts[2], out int dishId))
            {
                return;
            }

            // update the database order's status.

            orderDatabaseHandler.UpdateOrderCurrentStatusById(orderId, parts[3]);

            string message = "cook_status;" + orderId + ";" + dishId + ";" + parts[3];
            foreach (SubscribtionModel subscribtion in cookSubscribtions)
            {
                if (subscribtion.ModelIds.Contains(orderId))
                {
                    await subscribtion.SendTextToClient(message);
                }
            }
            foreach (SubscribtionModel subscribtion in orderSubscribtions)
            {
                if (subscribtion.ModelIds.Contains(orderId))
                {
                    await subscribtion.SendTextToClient(message);
                }
            }
        }


        public async Task HandleWholeRequest(HttpContext context,
            OrderDatabaseHandler orderDatabaseHandler)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                await HandleWebSocket(context, orderDatabaseHandler);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        public async Task HandleWebSocket(HttpContext context,
            OrderDatabaseHandler orderDatabaseHandler)
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
                            HandleCookStatus(parts, orderDatabaseHandler);
                            break;
                        }
                }
            });

            if (path.EndsWith("/cook"))
            {
                cookSubscribtions.Add(subscribtionModel);

            }
            else if (path.EndsWith("/reservations"))
            {
                reservationSubscribtions.Add(subscribtionModel);

            }
            else if (path.EndsWith("/orders"))
            {
                orderSubscribtions.Add(subscribtionModel);
            }

            await socketFinishedTcs.Task;
        }
    }
}
