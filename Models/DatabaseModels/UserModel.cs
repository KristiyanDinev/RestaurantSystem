namespace RestaurantSystem.Models.DatabaseModels {
    public class UserModel {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Image { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly LastTimeLoggedIn { get; set; }


        public int? RestaurantId { get; set; }
        public RestaurantModel? Restaurant { get; set; }



        // navigation
        public ICollection<UserRoleModel> Roles { get; set; } = new List<UserRoleModel>();
        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
        public ICollection<ReservationModel> Reservations { get; set; } = new List<ReservationModel>();
        public ICollection<DeliveryModel> Deliveries { get; set; } = new List<DeliveryModel>();
        public ICollection<AddressModel> Addresses { get; set; } = new List<AddressModel>();

    }
}
