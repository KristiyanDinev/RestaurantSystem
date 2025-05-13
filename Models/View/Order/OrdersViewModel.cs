using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Order
{
    public class OrdersViewModel
    {
        public required UserModel User { get; set; }
        public required Dictionary<OrderModel, List<DishModel>> Orders { get; set; }
    }
}
