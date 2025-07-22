using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Utilities;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Enums;

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

        public async Task<OrderModel?> AddOrderAsync(long userId, int restaurantId,
            List<int> dishesId, string? notes, decimal totalPrice,
            string? tableNumber, string? cupon, long? address_id)
        {
            // total price here is with applied discount if the code is correct.
            OrderModel order = new OrderModel
            {
                Notes = notes != null && notes.Replace(" ", "").Length == 0 ? null : notes,
                RestaurantId = restaurantId,
                CurrentStatus = OrderStatusEnum.Pending,
                TotalPrice = decimal.Parse($"{totalPrice:F2}"),
                UserId = userId,
                TableNumber = tableNumber,
                UserAddressId = address_id,
                CuponCode = cupon
            };

            await _databaseContext.Orders.AddAsync(order);

            if (await _databaseContext.SaveChangesAsync() <= 0) {
                return null;
            }

            foreach (int id in dishesId)
            {
                await _orderedDishesDatabaseHandler.CreateOrderedDishAsync(id, order.Id);
            }

            if (await _databaseContext.SaveChangesAsync() <= 0)
            {
                return null;
            }

            return order;
        }

        public async Task<bool> DeleteOrderAsync(long orderId, 
            bool checkOrderStatus = true)
        {
            OrderModel? order = await _databaseContext.Orders.FirstOrDefaultAsync(
                o => o.Id == orderId);
            if (order == null)
            {
                return false;
            }

            if (checkOrderStatus && !order.CurrentStatus.Equals(OrderStatusEnum.Pending))
            {
                return false;
            }

            await _orderedDishesDatabaseHandler.DeleteOrderedDishesAsync(orderId);
            _databaseContext.Orders.Remove(order);
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateOrderCurrentStatusByIdAsync(long orderId, 
            OrderStatusEnum status)
        {
            OrderModel? order = await _databaseContext.Orders.FirstOrDefaultAsync(
                o => o.Id == orderId) ?? throw new Exception();
            if (order.CurrentStatus.Equals(status))
            {
                return true;
            }

            if ((order.CurrentStatus.Equals(OrderStatusEnum.Ready) &&
                ((order.TableNumber != null && status.Equals(OrderStatusEnum.Served)) ||
                 status.Equals(OrderStatusEnum.Preparing) ||
                (order.TableNumber == null && status.Equals(OrderStatusEnum.Delivering)))) ||

                 (order.CurrentStatus.Equals(OrderStatusEnum.Preparing) &&
                 (status.Equals(OrderStatusEnum.Pending) || status.Equals(OrderStatusEnum.Ready))) ||

                 (order.CurrentStatus.Equals(OrderStatusEnum.Pending) &&
                 status.Equals(OrderStatusEnum.Preparing)) ||

                 (order.CurrentStatus.Equals(OrderStatusEnum.Served) &&
                 status.Equals(OrderStatusEnum.Ready)) ||

                 (order.CurrentStatus.Equals(OrderStatusEnum.Delivering) &&
                   (status.Equals(OrderStatusEnum.Ready) || 
                   status.Equals(OrderStatusEnum.Delivered))) ||

                   (order.CurrentStatus.Equals(OrderStatusEnum.Delivered) &&
                   status.Equals(OrderStatusEnum.Ready)))
            {

                order.CurrentStatus = status;
                return await _databaseContext.SaveChangesAsync() >= 0;
            }
            return false;
        }

        public async Task<List<OrderModel>> GetOrdersByUserAsync(long userId)
        {
            return await _databaseContext.Orders
                .Include(order => order.Restaurant)
                .Include(order => order.UserAddress)
                .Include(order => order.OrderedDishes)
                .Include(order => order.Cupon)
                .Where(order => 
                order.UserId == userId && 
                order.TableNumber == null)
                .ToListAsync();
        }

        public async Task<List<OrderModel>> GetCookOrdersByRestaurantIdAsync(int restaurantId, int page)
        {
            return await Utility.GetPageAsync<OrderModel>(_databaseContext.Orders
                .Include(order => order.User)
                .Where(order =>
                order.RestaurantId == restaurantId &&
                !(order.CurrentStatus.Equals(OrderStatusEnum.Delivering) ||
                  order.CurrentStatus.Equals(OrderStatusEnum.Delivered)))
                .OrderByDescending(order => order.OrderedAt)
                .AsQueryable(), page)
                .ToListAsync();
        }

        public async Task<List<OrderModel>> GetWaitressOrdersByRestaurantIdAsync(int restaurantId, int page)
        {
            return await Utility.GetPageAsync<OrderModel>(_databaseContext.Orders
                .Include(order => order.User)
                .Include(order => order.Cupon)
                .Where(order =>
                order.RestaurantId == restaurantId &&
                !(order.CurrentStatus.Equals(OrderStatusEnum.Delivering) ||
                  order.CurrentStatus.Equals(OrderStatusEnum.Delivered)))
                .OrderByDescending(order => order.Id)
                .AsQueryable(), page)
                .ToListAsync();
        }

        public async Task<List<OrderModel>> GetDeliveryOrdersByRestaurantIdAsync(int restaurantId, int page)
        {
            return await Utility.GetPageAsync<OrderModel>(
                _databaseContext.Orders
                .Include(order => order.UserAddress)
                .Include(order => order.User)
                .Where(order =>
                order.RestaurantId == restaurantId &&
                order.TableNumber == null &&

                !(order.CurrentStatus.Equals(OrderStatusEnum.Delivering) ||

                 order.CurrentStatus.Equals(OrderStatusEnum.Delivered))
                )
                .OrderBy(res => res.OrderedAt), page)
                .ToListAsync();
        }

        public async Task<List<OrderModel>> Get_HomeDelivery_OrdersBy_RestaurantIdAsync(int restaurantId)
        {
            return await _databaseContext.Orders.Where(
                order => 
                order.RestaurantId == restaurantId &&
                order.TableNumber == null)
                .ToListAsync();
        }

        public async Task<OrderModel?> GetOrderByIdAsync(long orderId)
        {
            return await _databaseContext.Orders
                .Include(order => order.OrderedDishes)
                .Include(order => order.Restaurant)
                .FirstOrDefaultAsync(order => order.Id == orderId);
        }

        public async Task<List<OrderModel>> GetDeliveredOrdersAsync(int restaurantId, int page)
        {
            return await Utility.GetPageAsync<OrderModel>(_databaseContext.Orders
                .Include(order => order.UserAddress)
                .Include(order => order.User)
                .Include(order => order.Delivery)
                .Where(order =>
                order.RestaurantId == restaurantId &&
                order.TableNumber == null &&
                order.CurrentStatus.Equals(OrderStatusEnum.Delivered))
                .OrderBy(order => order.UserAddress.City)
                .AsQueryable(), page)
                .ToListAsync();
        }
    }
}
