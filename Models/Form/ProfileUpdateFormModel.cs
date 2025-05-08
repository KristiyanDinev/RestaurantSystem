namespace RestaurantSystem.Models.Form
{
    public class ProfileUpdateFormModel
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Notes { get; set; }
        public string? Image { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public string? State { get; set; }
        public required string Country { get; set; }
    }
}
