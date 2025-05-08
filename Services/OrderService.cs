using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class OrderService
    {

        private DatabaseContext _databaseContext;
        private OrderedDishesService _orderedDishesDatabaseHandler;
        private readonly string pendingStatus = "pending";

        public OrderService(DatabaseContext databaseContext, OrderedDishesService orderedDishesDatabaseHandler)
        {
            _databaseContext = databaseContext;
            _orderedDishesDatabaseHandler = orderedDishesDatabaseHandler;
        }

        public async Task<OrderModel> AddOrder(int userId, int restaurantId,
            List<int> dishesId, string? notes, decimal totalPrice, bool isHomeDelivery)
        {
            OrderModel order = new OrderModel {
                Notes = notes,
                RestaurantId = restaurantId,
                CurrentStatus = pendingStatus,
                TotalPrice = totalPrice,
                UserId = userId,
                IsHomeDelivery = isHomeDelivery
            };

            await _databaseContext.Orders.AddAsync(order);

            await _databaseContext.SaveChangesAsync();

            foreach (int id in dishesId)
            {
                await _orderedDishesDatabaseHandler.CreateOrderedDish(id, order.Id, null, false);
            }

            await _databaseContext.SaveChangesAsync();

            return order;
        }

        public async Task DeleteOrder(int orderId)
        {
            OrderModel? order = await _databaseContext.Orders.FirstOrDefaultAsync(
                o => o.Id == orderId) ?? throw new Exception();

            if (!order.CurrentStatus.Equals(pendingStatus))
            {
                throw new Exception();
            }

            await _orderedDishesDatabaseHandler.DeleteOrderedDishes(orderId);

            _databaseContext.Orders.Remove(order);

            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateOrderCurrentStatusById(int orderId, string status)
        {
            OrderModel? order = await _databaseContext.Orders.FirstOrDefaultAsync(
                o => o.Id == orderId) ?? throw new Exception();

            order.CurrentStatus = status;

            await _databaseContext.SaveChangesAsync();
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
    }
}
