using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Database.Handlers
{
    public class OrderDatabaseHandler
    {

        private DatabaseManager _databaseManager;
        private OrderedDishesDatabaseHandler _orderedDishesDatabaseHandler;

        public OrderDatabaseHandler(DatabaseManager databaseManager, OrderedDishesDatabaseHandler orderedDishesDatabaseHandler)
        {
            _databaseManager = databaseManager;
            _orderedDishesDatabaseHandler = orderedDishesDatabaseHandler;
        }

        public async Task AddOrder(int userId, int restaurantId,
            List<int> dishesId, string? notes, decimal totalPrice)
        {
            OrderModel order = new OrderModel();
            order.Notes = notes;
            order.RestaurantModelId = restaurantId;
            order.TotalPrice = totalPrice;
            order.UserModelId = userId;

            await _databaseManager.Orders.AddAsync(order);

            await _databaseManager.SaveChangesAsync();

            foreach (int id in dishesId)
            {
                await _orderedDishesDatabaseHandler.CreateOrderedDish(id, order.Id, null, false);
            }

            await _databaseManager.SaveChangesAsync();
        }

        public async Task DeleteOrder(int orderId)
        {
            OrderModel? order = await _databaseManager.Orders.FirstOrDefaultAsync(
                o => o.Id == orderId);

            if (order == null)
            {
                return;
            }

            await _orderedDishesDatabaseHandler.DeleteOrderedDishes(orderId);

            _databaseManager.Orders.Remove(order);

            await _databaseManager.SaveChangesAsync();
        }

        public async Task UpdateOrderCurrentStatusById(int orderId, string status)
        {
            OrderModel? order = await _databaseManager.Orders.FirstOrDefaultAsync(
                o => o.Id == orderId);

            if (order == null)
            {
                return;
            }

            order.CurrentStatus = status;

            await _databaseManager.SaveChangesAsync();
        }

        public async Task<List<OrderModel>> GetOrdersByUser(int userId)
        {
            return await _databaseManager.Orders.Where(
                order => order.UserModelId == userId)
                .ToListAsync();
        }

        public async Task<OrderModel?> GetOrderById(int orderId)
        {
            return await _databaseManager.Orders.FirstOrDefaultAsync(order => order.Id == orderId);
        }
    }
}
