using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models
{
    public class RestaurantWithOrdersModel
    {
        public required RestaurantModel Restaurant { get; set; }
        public required List<OrderModel> Orders { get; set; }
    }
}
