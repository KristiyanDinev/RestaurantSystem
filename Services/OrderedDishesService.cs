using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Enums;
using RestaurantSystem.Models;
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

        public async Task<OrderedDishesModel> CreateOrderedDishAsync(int dishModelId, 
            long orderModelId)
        {
            OrderedDishesModel orderedDishes = new OrderedDishesModel()
            {
                OrderId = orderModelId,
                DishId = dishModelId,
                CurrentStatus = DishStatusEnum.Pending
            };

            await _databaseContext.OrderedDishes.AddAsync(orderedDishes);
            
            return orderedDishes;
        }


        public async Task<bool> UpdateOrderedDishStatusByIdAsync(int dishId, 
            long orderId, DishStatusEnum status)
        {
            List<OrderedDishesModel> orderedDishes = await _databaseContext.OrderedDishes
                .Where(order => order.DishId == dishId && order.OrderId == orderId)
                .ToListAsync();

            if (orderedDishes == null)
            {
                return false;
            }

            foreach (OrderedDishesModel orderedDish in orderedDishes)
            {
                orderedDish.CurrentStatus = status;
            }

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteOrderedDishesAsync(long orderId)
        {
            List<OrderedDishesModel> dishes = await _databaseContext.OrderedDishes
                .Where(order => order.OrderId == orderId)
                .ToListAsync();

            foreach (OrderedDishesModel orderedDishes in dishes)
            {
                _databaseContext.OrderedDishes.Remove(orderedDishes);
            }

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<List<DishStatusEnum>> GetDishCurrectStatusAsync(long orderId)
        {
            return await _databaseContext.OrderedDishes
                .Where(order => order.OrderId == orderId)
                .Select(orderedDish => orderedDish.CurrentStatus)
                .ToListAsync();
        }

        /*
         * Converts OrderedDishes to a list of dishes.
         */
        public async Task<Dictionary<DishWithStatusModel, int>> CountDishesByOrderAsync(long orderId)
        {
            List<OrderedDishesModel> orderedDishes = await _databaseContext.OrderedDishes.Where(
                order => order.OrderId == orderId)
                .ToListAsync();
            List<int> IDs = new List<int>();
            foreach (OrderedDishesModel orderedDish in orderedDishes) {
                IDs.Add(orderedDish.DishId);
            }

            List<DishModel> dishes = await _databaseContext.Dishies.Where(
                dish => IDs.Contains(dish.Id))
                .ToListAsync();
            Dictionary<DishWithStatusModel, int> result = new ();
            foreach (int id in IDs)
            {
                OrderedDishesModel orderedDish = orderedDishes
                    .Find(orderedDishes => orderedDishes.DishId == id)!;
                DishModel? dish = dishes.Find(dish => dish.Id == id);
                if (dish == null)
                {
                    continue;
                }

                DishWithStatusModel dishWithStatus = new DishWithStatusModel()
                {
                    Dish = dish,
                    OrderedDish = orderedDish
                };
                if (result.ContainsKey(dishWithStatus))
                {
                    result[dishWithStatus] = ++result[dishWithStatus];

                } else
                {
                    result[dishWithStatus] = 1;
                }
            }

            return result;
        }
    }
}
