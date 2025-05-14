namespace RestaurantSystem.Models.DatabaseModels
{
    public class RoleModel
    {
        public required string Name { get; set; }  // PK
        public string? Description { get; set; }


        // Navigation properties
        public ICollection<UserRoleModel> UserRoles { get; set; } = new List<UserRoleModel>();
        public ICollection<RolePermissionModel> RolePermissions { get; set; } = new List<RolePermissionModel>();
    }
}
