using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Utilities;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class OrderService
    {

        private DatabaseContext _databaseContext;
        private OrderedDishesService _orderedDishesDatabaseHandler;

        public OrderService(DatabaseContext databaseContext, 
            OrderedDishesService orderedDishesDatabaseHandler,
            WebSocketUtility webSocketUtility)
        {
            _databaseContext = databaseContext;
            _orderedDishesDatabaseHandler = orderedDishesDatabaseHandler;
        }

        public async Task<OrderModel?> AddOrder(int userId, int restaurantId,
            List<int> dishesId, string? notes, decimal totalPrice,
            string? tableNumber, string? cupon_code)
        {
            // total price here is with applied discount if the code is correct.
            OrderModel order = new OrderModel {
                Notes = notes != null && notes.Replace(" ", "").Length == 0 ? null : notes,
                RestaurantId = restaurantId,
                CurrentStatus = _databaseContext.DefaultOrder_CurrentStatus,
                TotalPrice = totalPrice,
                UserId = userId,
                TableNumber = tableNumber
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

            if (order == null || !order.CurrentStatus.Equals(
                _databaseContext.DefaultOrder_CurrentStatus))
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
            return await _databaseContext.Orders
                .Include(order => order.Restaurant)
                .Include(order => order.OrderedDishes)
                .Where(
                order => order.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<OrderModel>> GetOrdersByRestaurantId(int restaurantId)
        {
            return await _databaseContext.Orders.Where(
                order => order.RestaurantId == restaurantId)
                .ToListAsync();
        }

        public async Task<List<OrderModel>> Get_HomeDelivery_OrdersBy_RestaurantId(int restaurantId)
        {
            return await _databaseContext.Orders.Where(
                order => 
                order.RestaurantId == restaurantId &&
                order.TableNumber == null)
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
