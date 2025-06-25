using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Delivery
{
    public class DeliveryOrdersViewModel
    {
        public required UserModel Staff { get; set; }
        public required List<OrderWithDishesCountModel> Orders { get; set; }
        public required RestaurantModel Restaurant { get; set; }
    }
}
