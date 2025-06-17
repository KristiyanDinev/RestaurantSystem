namespace RestaurantSystem.Models.DatabaseModels
{
    public class AddressModel
    {
        public long Id { get; set; }
        
        public long UserId { get; set; }
        public UserModel User { get; set; } = null!;

        public required string Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string PostalCode { get; set; }
        public string? Notes { get; set; }

        // navigation
        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
    }
}
