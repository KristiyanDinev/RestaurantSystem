namespace RestaurantSystem.Models.DatabaseModels
{
    public class RestaurantModel
    {
        public int Id { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public string? State { get; set; }
        public required string Country { get; set; }
        public required string PostalCode { get; set; }
        public bool DoDelivery { get; set; }
        public bool ServeCustomersInPlace { get; set; }

        public required int ReservationMaxChildren { get; set; }
        public required int ReservationMinChildren { get; set; }
        public required int ReservationMaxAdults { get; set; }
        public required int ReservationMinAdults { get; set; }

        public required decimal Price_Per_Adult { get; set; }

        public required decimal Price_Per_Children { get; set; }


        // navigation
        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
        public ICollection<ReservationModel> Reservations { get; set; } = new List<ReservationModel>();
        public ICollection<UserModel> Employees { get; set; } = new List<UserModel>();
    }
}
