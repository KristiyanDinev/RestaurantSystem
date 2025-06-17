using RestaurantSystem.Models.WebSockets;
using System.Net.WebSockets;

namespace RestaurantSystem.Utilities
{
    public class WebSocketUtility
    {
        private List<OrderWebSocketModel> OrderWebSockets = new List<OrderWebSocketModel>();

        public WebSocketUtility()
        {
        }

        public void AddOrdersToListenTo(List<long> orderIds, WebSocket socket)
        {
            OrderWebSockets.RemoveAll(order => !order.Socket.State.Equals(WebSocketState.Open));

            OrderWebSockets.Add(new OrderWebSocketModel()
            {
                Socket = socket,
                OrderIds = orderIds
            });
        }

        public List<WebSocket> GetListenersForOrderId(long order_id)
        {
            return OrderWebSockets
                .Where(order => order.OrderIds.Contains(order_id))
                .Select(order => order.Socket).ToList();
        }
    }
}
