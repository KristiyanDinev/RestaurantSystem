using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Order
{
    public class OrdersViewModel
    {
        public string? StartedOrder { get; set; }

        public required UserModel User { get; set; }

        public required List<OrderWithDishesCountModel> Orders { get; set; }
    }
}
