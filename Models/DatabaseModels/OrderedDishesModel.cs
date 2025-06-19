using RestaurantSystem.Enums;

namespace RestaurantSystem.Models.DatabaseModels
{
    public class OrderedDishesModel
    {
        public long Id { get; set; }

        public required long OrderId { get; set; }
        public OrderModel Order { get; set; } = null!;

        public required int DishId { get; set; }
        public DishModel Dish { get; set; } = null!;

        public required DishStatusEnum CurrentStatus { get; set; }
    }
}
