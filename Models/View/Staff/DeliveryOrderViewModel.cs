using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff
{
    public class DeliveryOrderViewModel
    {
        public required UserModel Staff { get; set; }

        public required List<RestaurantWithOrdersModel> Order { get; set; } = new List<RestaurantWithOrdersModel>();
    }
}
