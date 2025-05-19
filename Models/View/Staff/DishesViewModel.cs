using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Admin
{
    public class DishesViewModel
    {
        public string? Error { get; set; }

        public RestaurantModel? Restaurant { get; set; }
        public UserModel? Staff { get; set; }

        public List<OrderWithDishesCountModel> OrderWithDishesCount { get; set; } = 
            new List<OrderWithDishesCountModel>();
    }
}
