namespace RestaurantSystem.Models.DatabaseModels
{
    public class UserRoleModel
    {
        public int UserId { get; set; }
        public UserModel User { get; set; }

        public string RoleName { get; set; } 
        public RoleModel Role { get; set; }
    }
}
