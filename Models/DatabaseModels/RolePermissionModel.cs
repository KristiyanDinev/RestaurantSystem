namespace RestaurantSystem.Models.DatabaseModels
{
    public class RolePermissionModel
    {
        public required string RoleName { get; set; }  // PK, FK
        public RoleModel Role { get; set; } = null!;


        public required string ServicePath { get; set; }  // PK, FK
        public ServiceModel Service { get; set; } = null!;
    }
}
