using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.User
{
    public class CartViewModel
    {
        public required UserModel User { get; set; }
        public required List<AddressModel> Addresses { get; set; }
        public required Dictionary<DishModel, int> Dishes { get; set; }
        public required RestaurantModel Restaurant { get; set; }
    }
}
