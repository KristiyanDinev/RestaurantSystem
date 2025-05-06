using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View
{
    public class DishesViewModel
    {
        public string? Error;

        public RestaurantModel? Restaurant;
        public UserModel? Staff;

        public Dictionary<OrderModel, List<DishModel>>? Dishes;
    }
}
