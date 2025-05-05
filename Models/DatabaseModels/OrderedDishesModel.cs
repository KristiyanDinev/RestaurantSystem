namespace RestaurantSystem.Models.DatabaseModels
{
    public class OrderedDishesModel
    {
        public int OrderId { get; set; }
        public OrderModel Order { get; set; }

        public int DishId { get; set; }
        public DishModel Dish { get; set; }

        public string? Notes { get; set; }
        public string CurrentStatus { get; set; }
    }
}
