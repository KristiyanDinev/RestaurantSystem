using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.User
{
    public class CartViewModel
    {
        public required UserModel User { get; set; }
        public required List<DishModel> Dishes { get; set; }
        public required RestaurantModel Restaurant { get; set; }
    }
}
