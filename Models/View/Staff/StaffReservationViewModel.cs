using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff
{
    public class StaffReservationViewModel
    {

        public required UserModel Staff { get; set; }

        public List<ReservationModel> Reservations { get; set; } = new List<ReservationModel>();
    }
}
