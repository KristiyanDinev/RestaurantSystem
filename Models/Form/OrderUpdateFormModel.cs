using RestaurantSystem.Enums;

namespace RestaurantSystem.Models.Form
{
    public class OrderUpdateFormModel
    {
        public required long OrderId { get; set; }

        public string? OrderCurrentStatus { get; set; }

        public required int DishId { get; set; }
        public string? DishCurrentStatus { get; set; }
    }
}
