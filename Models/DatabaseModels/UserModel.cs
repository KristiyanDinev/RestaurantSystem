namespace RestaurantSystem.Models.DatabaseModels {
    public class UserModel {

        public int Id { get; set; }

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
        public required string PostalCode { get; set; }
        public DateTime CreatedAt { get; set; }


        public int? RestaurantId { get; set; }
        public RestaurantModel? Restaurant { get; set; }



        // navigation
        public ICollection<UserRoleModel> Roles { get; set; } = new List<UserRoleModel>();
        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
        public ICollection<ReservationModel> Reservations { get; set; } = new List<ReservationModel>();
        public DeliveryModel Delivery { get; set; } = null!;

    }
}
