namespace RestaurantSystem.Models.Form
{
    public class AddressUpdateFormModel
    {
        public required long Id { get; set; }
        public required string Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string PostalCode { get; set; }
        public string? Notes { get; set; }
    }
}
