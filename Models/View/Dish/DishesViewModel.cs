using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Dish
{
    public class DishesViewModel
    {
        public required UserModel User { get; set; }
        public required RestaurantModel Restaurant { get; set; }
    }
}
