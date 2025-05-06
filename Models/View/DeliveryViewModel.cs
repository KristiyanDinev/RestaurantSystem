using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View
{
    public class DeliveryViewModel
    {

        public string? Error;

        public UserModel? Staff;

        public Dictionary<RestaurantModel, List<OrderModel>>? Orders;
    }
}
