using RestaurantSystem.Enums;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.User
{
    public class NavBarViewModel
    {
        public required UserModel User { get; set; }
        public required List<RoleEnum> Roles { get; set; }
    }
}
