using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Reservation
{


    // This is the model which will be in the
    // View where the user will have to fill up the
    // form to make the reservation.
    // The endpoint already requires the user to be logged in and 
    // to have selected a restaurant.
    public class ReservationFormViewModel
    {
        public required UserModel User { get; set; }

        public required RestaurantModel Restaurant { get; set; }
    }
}
