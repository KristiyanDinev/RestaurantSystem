using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.WebSockets;
using System.Net.WebSockets;

namespace RestaurantSystem.Services
{
    public class OrderService
    {

        private DatabaseContext _databaseContext;
        private OrderedDishesService _orderedDishesDatabaseHandler;
        private readonly string PendingStatus = "pending";

        private List<OrderWebSocketModel> OrderWebSockets;

        public OrderService(DatabaseContext databaseContext, OrderedDishesService orderedDishesDatabaseHandler)
        {
            _databaseContext = databaseContext;
            _orderedDishesDatabaseHandler = orderedDishesDatabaseHandler;
            OrderWebSockets = new List<OrderWebSocketModel>();
        }

        public async Task<OrderModel?> AddOrder(int userId, int restaurantId,
            List<int> dishesId, string? notes, decimal totalPrice, bool isHomeDelivery)
        {
            OrderModel order = new OrderModel {
                Notes = notes,
                RestaurantId = restaurantId,
                CurrentStatus = PendingStatus,
                TotalPrice = totalPrice,
                UserId = userId,
                IsHomeDelivery = isHomeDelivery
            };

            await _databaseContext.Orders.AddAsync(order);

            if (await _databaseContext.SaveChangesAsync() <= 0) {
                return null;
            }

            foreach (int id in dishesId)
            {
                await _orderedDishesDatabaseHandler.CreateOrderedDish(id, order.Id, null);
            }

            if (await _databaseContext.SaveChangesAsync() <= 0)
            {
                return null;
            }

            return order;
        }

        public async Task<bool> DeleteOrder(int orderId)
        {
            OrderModel? order = await _databaseContext.Orders.FirstOrDefaultAsync(
                o => o.Id == orderId);

            if (order == null || !order.CurrentStatus.Equals(PendingStatus))
            {
                return false;
            }

            await _orderedDishesDatabaseHandler.DeleteOrderedDishes(orderId);

            _databaseContext.Orders.Remove(order);

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateOrderCurrentStatusById(int orderId, string status)
        {
            OrderModel? order = await _databaseContext.Orders.FirstOrDefaultAsync(
                o => o.Id == orderId) ?? throw new Exception();

            order.CurrentStatus = status;

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<List<OrderModel>> GetOrdersByUser(int userId)
        {
            return await _databaseContext.Orders.Where(
                order => order.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<OrderModel>> GetOrdersByRestaurantId(int restaurantId)
        {
            return await _databaseContext.Orders.Where(
                order => order.RestaurantId == restaurantId)
                .ToListAsync();
        }

        public async Task<List<OrderModel>> GetOrdersByRestaurantId_WithHomeDeliveryOption(int restaurantId, bool isHomeDelivery)
        {
            return await _databaseContext.Orders.Where(
                order => order.RestaurantId == restaurantId && 
                order.IsHomeDelivery.Equals(isHomeDelivery))
                .ToListAsync();
        }

        public async Task<OrderModel?> GetOrderById(int orderId)
        {
            return await _databaseContext.Orders
                .Include(order => order.OrderedDishes)
                .Include(order => order.Restaurant)
                .FirstOrDefaultAsync(order => order.Id == orderId);
        }

        public void AddOrdersToListenTo(int user_id, List<int> orderIds, WebSocket socket)
        {
            if (OrderWebSockets.Any(order => order.UserId == user_id))
            {
                return;
            }

            OrderWebSockets.Add(new OrderWebSocketModel()
            {
                UserId = user_id,
                Socket = socket,
                OrderIds = orderIds
            });
        }

        public void RemoveOrderFromTracking(int user_id)
        {
            OrderWebSockets.RemoveAll(order => order.UserId == user_id);
        }

        public List<WebSocket> CheckIfOrderIsBeingTracked(int user_id, int order_id)
        {
            OrderWebSocketModel? orderWebSocketModel = OrderWebSockets
                .FirstOrDefault(order => order.UserId == user_id);

            if (orderWebSocketModel == null || orderWebSocketModel.OrderIds == null)
            {
                return false;
            }

            return orderWebSocketModel.OrderIds.Any(orderId => orderId == order_id);
        }

    }
}
