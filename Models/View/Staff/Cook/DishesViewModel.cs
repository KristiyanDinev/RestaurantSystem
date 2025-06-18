using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Cook
{
    public class DishesViewModel
    {
        public required UserModel Staff { get; set; }

        public List<OrderWithDishesCountModel> Orders { get; set; } = 
            new List<OrderWithDishesCountModel>();
    }
}
