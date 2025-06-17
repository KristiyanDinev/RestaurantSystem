namespace RestaurantSystem.Models.Form
{
    public class OrderFormModel
    {
        public string? Notes { get; set; }
        public required long AddressId { get; set; }
        public string? CuponCode { get; set; }
    }
}
