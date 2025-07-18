using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Waitress
{
    public class OrderDetailsViewModel
    {
        public required UserModel Staff { get; set; }
        public required Dictionary<DishModel, int> Dishes { get; set; }
    }
}
