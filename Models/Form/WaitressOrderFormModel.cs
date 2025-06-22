namespace RestaurantSystem.Models.Form
{
    public class WaitressOrderFormModel
    {
        public string? Notes { get; set; }
        public string? CuponCode { get; set; }
        public required string TableNumber { get; set; }
    }
}
