using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models
{
    public class OrderWithDishesCountModel
    {
        public required OrderModel Order { get; set; }

        // Dishes with their count and status
        public required Dictionary<DishWithStatusModel, int> DishesCount { get; set; }
    }
}
