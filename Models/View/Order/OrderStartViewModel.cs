using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Order
{
    public class OrderStartViewModel
    {
        public bool Success { get; set; } = false;
        public UserModel? User { get; set; }
        public RestaurantModel? Restaurant { get; set; }
        public OrderModel? Order { get; set; }
    }
}
