using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Order
{
    public class OrderStopView
    {
        public bool Success { get; set; } = false;
        public UserModel? User { get; set; }
    }
}
