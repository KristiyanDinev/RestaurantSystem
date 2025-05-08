using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Dish
{
    public class DishesTypeViewModel
    {

        public RestaurantModel? Restaurant { get; set; }
        public List<DishModel>? Dishes { get; set; }
    }
}
