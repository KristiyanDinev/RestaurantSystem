using System.Net.WebSockets;

namespace RestaurantSystem.Models.WebSockets
{
    public class OrderWebSocketModel
    {
        public required WebSocket Socket { get; set; }
        public required List<int> OrderIds { get; set; }
    }
}
