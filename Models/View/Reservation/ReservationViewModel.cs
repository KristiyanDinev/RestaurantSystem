using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Reservation
{
    public class ReservationViewModel
    {
        public UserModel? User { get; set; }
        public ReservationModel? Reservation { get; set; }
    }
}
