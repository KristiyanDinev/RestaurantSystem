using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Order
{
    public class OrderStartViewModel
    {
        public bool Success { get; set; } = false;
        public UserModel? User { get; set; }
        public RestaurantModel? Restaurant { get; set; }


        // Order will be null if the order didn't start 
        public OrderModel? Order { get; set; }
    }
}
