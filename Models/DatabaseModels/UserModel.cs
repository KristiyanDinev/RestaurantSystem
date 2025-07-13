namespace RestaurantSystem.Models.DatabaseModels {
    public class UserModel {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateOnly LastTimeLogedIn { get; set; }


        public int? RestaurantId { get; set; }
        public RestaurantModel? Restaurant { get; set; }



        // navigation
        public ICollection<UserRoleModel> Roles { get; set; } = new List<UserRoleModel>();
        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
        public ICollection<ReservationModel> Reservations { get; set; } = new List<ReservationModel>();
        public DeliveryModel Delivery { get; set; } = null!;
        public ICollection<AddressModel> Addresses { get; set; } = new List<AddressModel>();

    }
}
