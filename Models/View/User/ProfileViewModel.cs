using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.User
{
    public class ProfileViewModel
    {
        // if it is not null, then it did update successfully
        public string? UpdatedSuccessfully { get; set; }

        public UserModel? User { get; set; }
    }
}
