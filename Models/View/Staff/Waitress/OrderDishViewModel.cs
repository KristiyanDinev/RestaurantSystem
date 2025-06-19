using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Waitress
{
    public class OrderDishViewModel
    {
        public required List<DishModel> Dishes { get; set; }
        public required string InfoMessage { get; set; }
    }
}
