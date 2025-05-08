using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.User
{
    public class CartViewModel
    {
        public UserModel? User { get; set; }
        public List<DishModel>? Dishes { get; set; }
    }
}
