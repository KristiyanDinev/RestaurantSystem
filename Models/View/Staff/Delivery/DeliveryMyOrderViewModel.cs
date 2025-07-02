using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Delivery
{
    public class DeliveryMyOrderViewModel
    {
        public required UserModel Staff { get; set; }
        public required OrderWithDishesCountModel Order { get; set; }
    }
}
