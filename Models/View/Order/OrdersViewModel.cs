using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Order
{
    public class OrdersViewModel
    {
        public UserModel? User { get; set; }
        public List<OrderModel>? Orders { get; set; }
    }
}
