using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Enums;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Services
{
    public class DishService
    {
        private DatabaseContext _databaseContext;

        public DishService(DatabaseContext databaseContext) {
            _databaseContext = databaseContext;
        }

        public async Task<List<DishModel>> GetDishesByTypeAndRestaurantIdAsync(DishTypeEnum type, int restaurantId)
        {
            return await _databaseContext.Dishies.Where(
                dish => dish.Type_Of_Dish.Equals(type) && 
                dish.RestaurantId == restaurantId)
                .ToListAsync();
        }

        public async Task<DishModel?> GetDishByIdAsync(int id) {
            return await _databaseContext.Dishies.FirstOrDefaultAsync(dish => dish.Id == id);
        }
        public async Task<List<DishModel>> GetDishesByIdsAsync(HashSet<int> IDs)
        {
            return IDs.Count == 0 ? new List<DishModel>() : await _databaseContext.Dishies.Where(
                dish => IDs.Contains(dish.Id))
                .ToListAsync();
        }

        public async Task<bool> CreateDishAsync(CreateDishFormModel dishFormModel,
            int restaurantModelId)
        {
            if (!DishTypeEnum.TryParse(dishFormModel.Type, out DishTypeEnum type))
            {
                return false;
            }
            await _databaseContext.Dishies.AddAsync(new DishModel()
            {
                Name = dishFormModel.Name,
                Type_Of_Dish = type,
                Price = dishFormModel.Price,
                RestaurantId = restaurantModelId,
                Ingredients = dishFormModel.Ingredients,
                AverageTimeToCook = dishFormModel.AverageTimeToCook,
                Grams = dishFormModel.Grams,
                IsAvailable = dishFormModel.IsAvailable,
                Image = await Utility.UploadDishImageAsync(dishFormModel.Image)
            });
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteDishAsync(int id)
        {
            DishModel? dish = await _databaseContext.Dishies
                .FirstOrDefaultAsync(d => d.Id == id);
            if (dish == null) { return false; }
            _databaseContext.Dishies.Remove(dish);
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public List<int> GetDishIDsFromCartAsync(HttpContext context)
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

        public async Task<List<DishModel>> GetDishesByRestaurantIdAsync(int restaurantId, int page)
        {
            List<DishModel>? dishes = await Utility.GetPageAsync<DishModel>(
                _databaseContext.Dishies
                .Where(dish => dish.RestaurantId == restaurantId)
                .OrderBy(dish => dish.Name)
                .AsQueryable(), page)
                .ToListAsync();
            return dishes ?? new List<DishModel>();
        }

        public async Task<bool> UpdateDishAsync(EditDishFormModel editDishFormModel)
        {
            if (!DishTypeEnum.TryParse(editDishFormModel.Type, out DishTypeEnum type))
            {
                return false;
            }
            DishModel? dish = await GetDishByIdAsync(editDishFormModel.Id);
            if (dish == null) { return false; }
            if (editDishFormModel.DeleteImage)
            {
                Utility.DeleteImage(dish.Image);
                dish.Image = null;

            }
            else
            {
                string? img = await Utility.UpdateImageAsync(dish.Image,
                    editDishFormModel.Image, true);
                if (img != null)
                {
                    dish.Image = img;
                }
            }
            dish.Name = editDishFormModel.Name;
            dish.Price = editDishFormModel.Price;
            dish.Grams = editDishFormModel.Grams;
            dish.Ingredients = editDishFormModel.Ingredients;
            dish.AverageTimeToCook = editDishFormModel.AverageTimeToCook;
            dish.Type_Of_Dish = type;
            dish.IsAvailable = editDishFormModel.IsAvailable;
            return await _databaseContext.SaveChangesAsync() > 0;
        }
    }
}
