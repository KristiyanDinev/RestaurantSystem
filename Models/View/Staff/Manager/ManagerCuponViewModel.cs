using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Manager
{
    public class ManagerCuponViewModel
    {
        public required UserModel Staff { get; set; }
        public required int Page { get; set; } = 1;
        public required List<CuponModel> Cupons { get; set; }
    }
}
