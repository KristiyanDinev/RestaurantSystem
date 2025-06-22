using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Waitress
{
    public class OrderChoseDishViewModel
    {
        public required UserModel Staff { get; set; }
        public required List<DishModel> Salads { get; set; } = new List<DishModel>();
        public required List<DishModel> Soups { get; set; } = new List<DishModel>();
        public required List<DishModel> Appetizers { get; set; } = new List<DishModel>();
        public required List<DishModel> Dishes { get; set; } = new List<DishModel>();
        public required List<DishModel> Desserts { get; set; } = new List<DishModel>();
        public required List<DishModel> Drinks { get; set; } = new List<DishModel>();
    }
}
