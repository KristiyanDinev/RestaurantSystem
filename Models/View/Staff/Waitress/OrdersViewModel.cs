using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Waitress
{
    public class OrdersViewModel
    {
        public required UserModel Staff { get; set; }
        public required int Page { get; set; } = 1;
        public required List<OrderWithDishesCountModel> Orders { get; set; }
    }
}
