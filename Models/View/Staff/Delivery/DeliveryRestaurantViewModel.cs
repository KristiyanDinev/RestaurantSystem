using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Delivery
{
    public class DeliveryRestaurantViewModel
    {
        public required UserModel Staff { get; set; }
        public int Page { get; set; } = 1;
        public required List<RestaurantModel> Restaurants { get; set; }
    }
}
