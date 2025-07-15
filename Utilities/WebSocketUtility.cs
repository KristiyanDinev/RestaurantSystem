using RabbitMQ.Client;
using RestaurantSystem.Models.WebSockets;
using System.Net.WebSockets;

namespace RestaurantSystem.Utilities
{
    public class WebSocketUtility
    {
        private List<OrderWebSocketModel> OrderWebSockets = new List<OrderWebSocketModel>();
        private IConnection? _connection = null;
        private IChannel? _channel = null;
        public readonly string _serverId;
        private readonly string _exchangeName = "websocket_distribution";
        private readonly string _queueName;
        public WebSocketUtility() {
            _serverId = Environment.MachineName + "_" + Guid.NewGuid().ToString("N")[..8];
            _queueName = $"websocket_queue_{_serverId}";
        }

        public List<OrderWebSocketModel> GetOrderWebSocketModels()
        {
            return OrderWebSockets;
        }

        public void SetConnection(IConnection connection)
        {
            _connection = connection;
        }

        public void SetChannel(IChannel channel)
        {
            _channel = channel;
        }

        public string GetServerId()
        {
            return _serverId;
        }

        public IConnection? GetConnection()
        {
            return _connection;
        }

        public IChannel? GetChannel()
        {
            return _channel;
        }

        public string GetExchangeName()
        {
            return _exchangeName;
        }

        public string GetQueueName()
        {
            return _queueName;
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


        public List<long> GetOrderIdsForSocket(WebSocket socket)
        {
            return OrderWebSockets
                .Where(order => order.Socket.Equals(socket))
                .SelectMany(order => order.OrderIds).ToList();
        }

        public void RemoveOrderIdsFromSocket(WebSocket socket)
        {
            OrderWebSockets.RemoveAll(order => order.Socket.Equals(socket));
        }
    }
}
