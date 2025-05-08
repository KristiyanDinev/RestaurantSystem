using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View
{
    public class CartViewModel
    {
        public UserModel? User { get; set; }
        public List<DishModel>? Dishes { get; set; }
    }
}
