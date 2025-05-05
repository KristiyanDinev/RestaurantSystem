using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class DishService
    {
        private DatabaseContext _databaseManager;
        public DishService(DatabaseContext databaseManager) {
            _databaseManager = databaseManager;
        }

        public async Task<List<DishModel>> GetDishesByTypeAndRestaurantId(string type, int restaurantId)
        {
            return await _databaseManager.Dishies.Where(
                dish => dish.Type_Of_Dish.Equals(type) && dish.RestaurantId == restaurantId)
                .ToListAsync();
        }

        public async Task<List<DishModel>> GetDishesByIds(List<int> IDs)
        {
            return await _databaseManager.Dishies.Where(
                dish => IDs.Contains(dish.Id))
                .ToListAsync();
        }

        public async Task<DishModel> CreateDish(string name, string type,
            decimal price, int restaurantModelId, string ingredients, string avrageTimeToCook, 
            int grams, string? notes, string? image, bool isAvailable)
        {
            DishModel dish = new DishModel();
            dish.Name = name;
            dish.Type_Of_Dish = type;
            dish.Price = price;
            dish.RestaurantId = restaurantModelId;
            dish.Ingredients = ingredients;
            dish.AvrageTimeToCook = avrageTimeToCook;
            dish.Grams = grams;
            dish.IsAvailable = isAvailable;
            dish.Notes = notes;
            dish.Image = image;

            await _databaseManager.Dishies.AddAsync(dish);

            await _databaseManager.SaveChangesAsync();

            return dish;
        }

        public async Task DeleteDish(int id)
        {
            DishModel? dish = await _databaseManager.Dishies.FirstOrDefaultAsync(
                d => d.Id == id) ?? throw new Exception("Dish doesn't exist.");

            _databaseManager.Dishies.Remove(dish);

            await _databaseManager.SaveChangesAsync();
        }

        public List<int> GetDishIDsFromCart(HttpContext context)
        {
            context.Request.Cookies.TryGetValue("cart", out string? cart);
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
