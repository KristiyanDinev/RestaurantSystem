namespace RestaurantSystem.Models.DatabaseModels
{
    public class OrderedDishesModel
    {
        public int Id { get; set; }

        public required int OrderId { get; set; }
        public OrderModel Order { get; set; } = null!;

        public required int DishId { get; set; }
        public DishModel Dish { get; set; } = null!;

        public string? Notes { get; set; }
        public required string CurrentStatus { get; set; }
    }
}
