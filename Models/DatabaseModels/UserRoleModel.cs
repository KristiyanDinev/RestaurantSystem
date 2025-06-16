namespace RestaurantSystem.Models.DatabaseModels
{
    public class UserRoleModel
    {
        public int Id { get; set; }
        public required long UserId { get; set; }
        public UserModel User { get; set; } = null!;

        public required string RoleName { get; set; }
        public RoleModel Role { get; set; } = null!;
    }
}
