using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Reservation
{
    public class ReservationDeleteViewModel
    {
        public UserModel? User { get; set; }
        public bool Success { get; set; } = false;
    }
}
