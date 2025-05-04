namespace RestaurantSystem.Models.DatabaseModels
{
    public class RestaurantModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string? State { get; set; }
        public string Country { get; set; }
        public bool DoDelivery { get; set; }
        public bool ServeCustomersInPlace { get; set; }

        public int ReservationMaxChildren { get; set; }
        public int ReservationMinChildren { get; set; }
        public int ReservationMaxAdults { get; set; }
        public int ReservationMinAdults { get; set; }

        public decimal Price_Per_Adult { get; set; }

        public decimal Price_Per_Children { get; set; }
    }
}
