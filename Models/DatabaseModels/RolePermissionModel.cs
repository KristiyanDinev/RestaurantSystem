namespace RestaurantSystem.Models.DatabaseModels
{
    public class RolePermissionModel
    {
        public string RoleName { get; set; }  // PK, FK
        public RoleModel Role { get; set; }


        public string ServicePath { get; set; }  // PK, FK
        public ServiceModel Service { get; set; }
    }
}
