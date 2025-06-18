using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Waitress
{
    public class OrdersViewModel
    {
        public required UserModel Staff { get; set; }
        public required List<OrderWithDishesCountModel> Orders { get; set; }
    }
}
