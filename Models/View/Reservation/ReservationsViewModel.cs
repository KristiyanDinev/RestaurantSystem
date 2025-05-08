using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Reservation
{
    public class ReservationsViewModel
    {
        public UserModel? User { get; set; }
        public List<ReservationModel>? Reservations { get; set; }
    }
}
