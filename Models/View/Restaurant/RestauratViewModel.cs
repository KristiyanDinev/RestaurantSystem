using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Restaurant
{
    public class RestaurantViewModel
    {
        public int Page { get; set; } = 1;
        public List<RestaurantModel> Restaurants { get; set; } = new List<RestaurantModel>();
    }
}
