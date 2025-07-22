namespace RestaurantSystem.Models.Form
{
    public class RestaurantEditModel
    {
        public required int Id { get; set; }
        public required string Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public required string Address { get; set; }
        public required string PostalCode { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public required int ReservationMaxAdults { get; set; }
        public required int ReservationMinAdults { get; set; }
        public required int ReservationMaxChildren { get; set; }
        public required int ReservationMinChildren { get; set; }

        public required bool DoDelivery { get; set; }
        public required bool ServeCustomersInPlace { get; set; }
    }
}
