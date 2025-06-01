using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Reservation
{
    public class ReservationsViewModel
    {
        public required UserModel User { get; set; }
        public required List<ReservationModel> Reservations { get; set; }
    }
}
