namespace RestaurantSystem.Models.DatabaseModels
{
    public class RolePermissionModel
    {
        public int Id { get; set; }

        public required string RoleName { get; set; }
        public required string ServicePath { get; set; }
        public ServiceModel Service { get; set; } = null!;
    }
}
