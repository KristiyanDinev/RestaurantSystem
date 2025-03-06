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


        public static void CloseAllOrderSubscribtion()
        {
            CloseAllSubscribtions(WebSocketController.orderSubscribtions);
        }

        public static void CloseAllReservationSubscribtion()
        {
            CloseAllSubscribtions(WebSocketController.reservationSubscribtions);
        }
    }
}
