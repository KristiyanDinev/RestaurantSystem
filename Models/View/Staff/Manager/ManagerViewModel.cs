using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Manager
{
    public class ManagerViewModel
    {
        public required UserModel Staff { get; set; }

        public required List<UserModel> Employees { get; set; }
    }
}
