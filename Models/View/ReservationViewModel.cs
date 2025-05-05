using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View
{
    public class ReservationViewModel
    {
        public string? Error;

        public UserModel? userModel;

        public List<ReservationModel>? reservations;
    }
}
