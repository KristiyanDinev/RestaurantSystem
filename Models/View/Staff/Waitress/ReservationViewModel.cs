using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Staff.Waitress
{
    public class ReservationViewModel
    {

        public required UserModel Staff { get; set; }

        public List<ReservationModel> Reservations { get; set; } = new List<ReservationModel>();
    }
}
