using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class OrderedDishesService
    {
        private DatabaseContext _databaseContext;

        public OrderedDishesService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<OrderedDishesModel> CreateOrderedDish(int dishModelId, 
            int orderModelId, string? notes)
        {
            OrderedDishesModel orderedDishes = new OrderedDishesModel()
            {
                OrderId = orderModelId,
                DishId = dishModelId,
                Notes = notes,
                CurrentStatus = _databaseContext.DefaultOrderedDish_CurrentStatus
            };

            await _databaseContext.OrderedDishes.AddAsync(orderedDishes);
            
            return orderedDishes;
        }


        public async Task<bool> UpdateOrderedDishStatusById(int dishId, int orderId, string status)
        {
            OrderedDishesModel? orderedDishes = await _databaseContext.OrderedDishes
                .FirstOrDefaultAsync(
                order => order.DishId == dishId && order.OrderId == orderId);

            if (orderedDishes == null)
            {
                return false;
            }

            orderedDishes.CurrentStatus = status;

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteOrderedDishes(int orderId)
        {
            List<OrderedDishesModel> dishes = await _databaseContext.OrderedDishes
                .Where(order => order.OrderId == orderId)
                .ToListAsync();

            foreach (OrderedDishesModel orderedDishes in dishes) {
                _databaseContext.OrderedDishes.Remove(orderedDishes);
            }

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<List<DishModel>> GetDishesFromOrder(int orderId)
        {
            List<OrderedDishesModel> orderedDishes = await _databaseContext.OrderedDishes.Where(
                order => order.OrderId == orderId)
                .ToListAsync();

            List<int> IDs = new List<int>();
            foreach (OrderedDishesModel orderedDish in orderedDishes) {
                IDs.Add(orderedDish.DishId);
            }

            return await _databaseContext.Dishies.Where(
                dish => 
                .ToListAsync();
        }
    }
}
