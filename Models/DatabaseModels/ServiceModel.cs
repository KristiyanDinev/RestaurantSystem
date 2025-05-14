namespace RestaurantSystem.Models.DatabaseModels
{
    public class ServiceModel
    {

        public required string Path { get; set; }  // PK
        public string? Description { get; set; }


        // Navigation property
        public ICollection<RolePermissionModel> RolePermissions { get; set; } = new List<RolePermissionModel>();

    }
}
