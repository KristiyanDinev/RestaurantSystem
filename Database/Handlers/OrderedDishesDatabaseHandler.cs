using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Database.Handlers
{
    public class OrderedDishesDatabaseHandler
    {
        private DatabaseManager _databaseManager;
        public OrderedDishesDatabaseHandler(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public async Task<OrderedDishesModel> CreateOrderedDish(int dishModelId, 
            int orderModelId, string? notes, bool saveChanges)
        {
            OrderedDishesModel orderedDishes = new OrderedDishesModel();
            orderedDishes.OrderModelId = orderModelId;
            orderedDishes.DishModelId = dishModelId;
            orderedDishes.Notes = notes;

            await _databaseManager.OrderedDishes.AddAsync(orderedDishes);

            if (saveChanges) { 
                await _databaseManager.SaveChangesAsync();
            }
            
            return orderedDishes;
        }


        public async Task UpdateOrderedDishStatusById(int dishId, int orderId, string status)
        {
            OrderedDishesModel? orderedDishes = await _databaseManager.OrderedDishes
                .FirstOrDefaultAsync(
                order => order.DishModelId == dishId && order.OrderModelId == orderId);

            if (orderedDishes == null)
            {
                return;
            }

            orderedDishes.CurrentStatus = status;

            await _databaseManager.SaveChangesAsync();
        }

        public async Task DeleteOrderedDishes(int orderId)
        {
            List<OrderedDishesModel> dishes = await _databaseManager.OrderedDishes
                .Where(order => order.OrderModelId == orderId)
                .ToListAsync();

            foreach (OrderedDishesModel orderedDishes in dishes) {
                _databaseManager.OrderedDishes.Remove(orderedDishes);
            }

            await _databaseManager.SaveChangesAsync();
        }

        public async Task<List<DishModel>> GetDishesFromOrder(int orderId)
        {
            List<OrderedDishesModel> orderedDishes = await _databaseManager.OrderedDishes.Where(
                order => order.OrderModelId == orderId)
                .ToListAsync();

            List<int> IDs = new List<int>();
            foreach (OrderedDishesModel orderedDish in orderedDishes) {
                IDs.Add(orderedDish.DishModelId);
            }

            return await _databaseManager.Dishies.Where(
                dish => IDs.Contains(dish.Id))
                .ToListAsync();
        }
    }
}
