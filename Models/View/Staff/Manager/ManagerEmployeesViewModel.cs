using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Manager
{
    public class ManagerEmployeesViewModel
    {
        public required UserModel Staff { get; set; }
        public required int Page { get; set; } = 1;
        public required List<UserModel> Employees { get; set; }
    }
}
