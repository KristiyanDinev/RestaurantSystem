using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models
{
    public class OrderWithDishesCountModel
    {
        public required OrderModel Order { get; set; }

        // Dishes with their count
        public required Dictionary<DishModel, int> DishesCount { get; set; }
    }
}
