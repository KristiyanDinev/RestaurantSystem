using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Manager
{
    public class ManagerDishViewModel
    {
        public required UserModel Staff { get; set; }
        public required List<DishModel> Dishes { get; set; }
        public required int Page { get; set; } = 1;
    }
}
