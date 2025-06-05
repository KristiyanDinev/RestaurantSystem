using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff
{
    public class DeliveryOrdersViewModel
    {

        public required UserModel Staff { get; set; }

        public required List<RestaurantWithOrdersModel> Orders { get; set; }
    }
}
