using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class DishService
    {
        private DatabaseContext _databaseContext;
        public DishService(DatabaseContext databaseContext) {
            _databaseContext = databaseContext;
        }

        public async Task<List<DishModel>> GetDishesByTypeAndRestaurantId(string type, int restaurantId)
        {
            return await _databaseContext.Dishies.Where(
                dish => dish.Type_Of_Dish.Equals(type) && dish.RestaurantId == restaurantId)
                .ToListAsync();
        }


        public async Task<DishModel?> GetDishById(int id) {
            return await _databaseContext.Dishies.FirstOrDefaultAsync(dish => dish.Id == id);
        }
        public async Task<List<DishModel>> GetDishesByIds(HashSet<int> IDs)
        {
            return IDs.Count == 0 ? new List<DishModel>() : await _databaseContext.Dishies.Where(
                dish => IDs.Contains(dish.Id))
                .ToListAsync();
        }

        public async Task<DishModel?> CreateDish(string name, string type,
            decimal price, int restaurantModelId, string ingredients, string avrageTimeToCook, 
            int grams, string? notes, string? image, bool isAvailable)
        {
            DishModel dish = new DishModel()
            {
                Name = name,
                Type_Of_Dish = type,
                Price = price,
                RestaurantId = restaurantModelId,
                Ingredients = ingredients,
                AvrageTimeToCook = avrageTimeToCook,
                Grams = grams,
                IsAvailable = isAvailable,
                Notes = notes,
                Image = image
            };

            await _databaseContext.Dishies.AddAsync(dish);

            return await _databaseContext.SaveChangesAsync() >= 1 ? dish : null;
        }

        public async Task<bool> DeleteDish(int id)
        {
            DishModel? dish = await _databaseContext.Dishies.FirstOrDefaultAsync(
                d => d.Id == id) ?? throw new Exception("Dish doesn't exist.");

            _databaseContext.Dishies.Remove(dish);

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public List<int> GetDishIDsFromCart(HttpContext context)
        {
            context.Request.Cookies.TryGetValue("cart_items", out string? cart);
            if (cart == null || cart.Length == 0)
            {
                return new List<int>();
            }

            List<int> dishesIds = new List<int>();

            foreach (string dishIdStr in cart.Split('-').ToList())
            {
                if (int.TryParse(dishIdStr, out int dishId))
                {
                    dishesIds.Add(dishId);
                }
            }
            return dishesIds;
        }
    }
}
