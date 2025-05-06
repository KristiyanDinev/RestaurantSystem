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
            int orderModelId, string? notes, bool saveChanges)
        {
            OrderedDishesModel orderedDishes = new OrderedDishesModel();
            orderedDishes.OrderId = orderModelId;
            orderedDishes.DishId = dishModelId;
            orderedDishes.Notes = notes;

            await _databaseContext.OrderedDishes.AddAsync(orderedDishes);

            if (saveChanges) { 
                await _databaseContext.SaveChangesAsync();
            }
            
            return orderedDishes;
        }


        public async Task UpdateOrderedDishStatusById(int dishId, int orderId, string status)
        {
            OrderedDishesModel? orderedDishes = await _databaseContext.OrderedDishes
                .FirstOrDefaultAsync(
                order => order.DishId == dishId && order.OrderId == orderId);

            if (orderedDishes == null)
            {
                return;
            }

            orderedDishes.CurrentStatus = status;

            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteOrderedDishes(int orderId)
        {
            List<OrderedDishesModel> dishes = await _databaseContext.OrderedDishes
                .Where(order => order.OrderId == orderId)
                .ToListAsync();

            foreach (OrderedDishesModel orderedDishes in dishes) {
                _databaseContext.OrderedDishes.Remove(orderedDishes);
            }

            await _databaseContext.SaveChangesAsync();
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
                dish => IDs.Contains(dish.Id))
                .ToListAsync();
        }
    }
}
