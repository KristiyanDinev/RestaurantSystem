namespace RestaurantSystem.Models.DatabaseModels
{
    public class ServiceModel
    {

        public string Service { get; set; }

        public ICollection<UserRoleModel> AllowedRoles { get; set; }
    }
}
