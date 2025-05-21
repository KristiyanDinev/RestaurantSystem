namespace RestaurantSystem.Models.Form
{
    public class RegisterFormModel
    {
        public required string Name { get; set; }
        public required string Password { get; set; }

        public string? Image { get; set; }

        public string? Notes { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public string? State { get; set; }
        public required string Country { get; set; }
        public bool RememberMe { get; set; }
        public required string PostalCode { get; set; }

    }
}
