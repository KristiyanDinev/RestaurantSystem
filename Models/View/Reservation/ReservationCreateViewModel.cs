using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Reservation
{
    public class ReservationCreateViewModel
    {
        public UserModel? User { get; set; }
        public ReservationModel? Reservation { get; set; }
        public RestaurantModel? Restaurant { get; set; }
    }
}
