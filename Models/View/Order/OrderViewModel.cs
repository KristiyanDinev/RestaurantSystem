using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Order
{
    public class OrderViewModel
    {
        public UserModel? User { get; set; }
        public OrderModel? Order { get; set; }
    }
}
