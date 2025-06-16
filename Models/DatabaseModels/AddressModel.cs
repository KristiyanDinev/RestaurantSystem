namespace RestaurantSystem.Models.DatabaseModels
{
    public class AddressModel
    {
        public int Id { get; set; }
        

        public int UserId { get; set; }
        public UserModel User { get; set; } = null!;

        public required string Country { get; set; }
        public string? State { get; set; }
        public required string City { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Notes { get; set; }
    }
}
