using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models
{
    public class UserWithOrderAndDishesModel
    {
        public required OrderWithDishesCountModel Order { get; set; }
        public required UserModel User { get; set; }
    }
}
