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

        public int ReservationMaxChildren { get; set; }
        public int ReservationMinChildren { get; set; }
        public int ReservationMaxAdults { get; set; }
        public int ReservationMinAdults { get; set; }

        public decimal Price_Per_Adult { get; set; }

        public decimal Price_Per_Children { get; set; }


        // navigation
        public ICollection<OrderModel> Orders { get; set; }
        public ICollection<ReservationModel> Reservations { get; set; }
        public ICollection<TimeTableModel> TimeTables { get; set; }
        public ICollection<UserModel> Employees { get; set; }
    }
}
