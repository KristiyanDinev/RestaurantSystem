namespace RestaurantSystem.Models.DatabaseModels
{
    public class RoleModel
    {
        public string Name { get; set; }  // PK
        public string? Description { get; set; }


        // Navigation properties
        public ICollection<UserRoleModel> UserRoles { get; set; }
        public ICollection<RolePermissionModel> RolePermissions { get; set; }
    }
}
