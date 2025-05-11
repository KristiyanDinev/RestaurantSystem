namespace RestaurantSystem.Models.Form
{
    public class OrderUpdateFormModel
    {
        public required int OrderId { get; set; }
        public string? OrderCurrentStatus { get; set; }

        public int? DishId { get; set; }
        public string? DishCurrentStatus { get; set; }
    }
}
