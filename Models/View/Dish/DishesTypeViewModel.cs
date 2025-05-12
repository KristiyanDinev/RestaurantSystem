using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Dish
{
    public class DishesTypeViewModel
    {
        public required RestaurantModel Restaurant { get; set; }
        public required UserModel User { get; set; }
        public required List<DishModel> Dishes { get; set; }
        public required string DishType { get; set; }
    }
}
