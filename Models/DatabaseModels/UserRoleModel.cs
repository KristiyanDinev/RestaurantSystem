namespace RestaurantSystem.Models.DatabaseModels
{
    public class UserRoleModel
    {
        public string Role { get; set; }

        public int UserModelId { get; set; }
        public UserModel UserModel { get; set; }

        public ICollection<ServiceModel> Services { get; set; }
    }
}
