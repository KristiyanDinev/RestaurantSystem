namespace RestaurantSystem.Models.DatabaseModels {
    public class UserModel {

        public int Id { get; set; }

        public string Name { get; set; }
        public string Password { get; set; }

        public string? Image { get; set; }

        public string? Notes { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; } 
        public string? State { get; set; } 
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<UserRoleModel> Roles { get; set; }
    }
}
