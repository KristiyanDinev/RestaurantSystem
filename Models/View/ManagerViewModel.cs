using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View
{
    public class ManagerViewModel
    {
        public string? Error;

        public UserModel? Staff;

        // The employees that work in that restaurant
        public List<UserModel>? Employees;

        public RestaurantModel? Restaurant;
    }
}
