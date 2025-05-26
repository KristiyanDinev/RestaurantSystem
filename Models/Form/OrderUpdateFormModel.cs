namespace RestaurantSystem.Models.Form
{
    public class OrderUpdateFormModel
    {
        public required int OrderId { get; set; }

        // formData.append("OrderCurrentStatus", '') will give null
        // formData.append("OrderCurrentStatus", null) will give "null"
        public string? OrderCurrentStatus { get; set; }

        public required int DishId { get; set; }
        public string? DishCurrentStatus { get; set; }
    }
}
