namespace RestaurantSystem.Models.DatabaseModels
{
    public class OrderedDishesModel
    {
        public int OrderModelId { get; set; }
        public OrderModel OrderModel { get; set; }

        public int DishModelId { get; set; }
        public DishModel DishModel { get; set; }

        public string? Notes { get; set; }
        public string CurrentStatus { get; set; }
    }
}
