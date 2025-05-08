using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Admin
{
    public class AdminReservationViewModel
    {
        public string? Error { get; set; }

        public UserModel? Staff { get; set; }

        public List<ReservationModel>? Reservations { get; set; }
    }
}
