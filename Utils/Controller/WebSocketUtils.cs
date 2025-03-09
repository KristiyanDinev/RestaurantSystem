using ITStepFinalProject.Controllers;
using ITStepFinalProject.Models;

namespace ITStepFinalProject.Utils.Controller
{
    public class WebSocketUtils
    {
        public WebSocketUtils() { }

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
            RemoveModelIdFromSubscribtion(id, WebSocketController.orderSubscribtions);
        }

        public void RemoveModelIdFromReservationSubscribtion(int id)
        {
            RemoveModelIdFromSubscribtion(id, WebSocketController.reservationSubscribtions);
        }

        public static void CloseAllSubscribtions()
        {
            CloseAllSubscribtions(WebSocketController.orderSubscribtions);
            CloseAllSubscribtions(WebSocketController.reservationSubscribtions);
            CloseAllSubscribtions(WebSocketController.cookSubscribtions);
        }

        public async static void HandleCookStatus(List<string> parts)
        {
            // 1  or  di s
            if (parts.Count != 4 || !int.TryParse(parts[1], out int orderId) ||
                                !int.TryParse(parts[2], out int dishId))
            {
                return;
            }

            // update the database order's status.

            string message = "cook_status;" + orderId + ";" + dishId + ";" + parts[3];
            foreach (SubscribtionModel subscribtion in WebSocketController.cookSubscribtions)
            {
                if (subscribtion.ModelIds.Contains(orderId))
                {
                    await subscribtion.SendTextToClient(message);
                }
            }
            foreach (SubscribtionModel subscribtion in WebSocketController.orderSubscribtions)
            {
                if (subscribtion.ModelIds.Contains(orderId))
                {
                    await subscribtion.SendTextToClient(message);
                }
            }
        }
    }
}
