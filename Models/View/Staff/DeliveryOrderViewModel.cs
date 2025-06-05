using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff
{
    public class DeliveryOrderViewModel
    {
        public required UserModel Staff { get; set; }

        public required RestaurantWithOrdersModel Order { get; set; }
    }
}
