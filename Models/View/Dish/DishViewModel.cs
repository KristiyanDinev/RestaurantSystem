using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Dish
{
    public class DishViewModel
    {
        public RestaurantModel? Restaurant { get; set; }
        public DishModel? Dish { get; set; }
    }
}
