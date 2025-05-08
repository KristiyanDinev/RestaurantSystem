using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Admin
{
    public class ManagerViewModel
    {
        public string? Error { get; set; }

        public UserModel? Staff { get; set; }

        public List<UserModel>? Employees { get; set; }

        public RestaurantModel? Restaurant { get; set; }
    }
}
