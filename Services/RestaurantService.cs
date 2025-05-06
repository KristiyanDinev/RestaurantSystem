using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class RestaurantService
    {

        private DatabaseContext _databaseContext;

        public RestaurantService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<TimeTableModel>> GetRestaurantsForDelivery_ForUser(UserModel user)
        {
            return await _databaseContext.TimeTables
                .Include(times => times.Restuarant)
                .Where(
                times => times.Restuarant.DoDelivery &&
                times.UserCity.Equals(user.City) &&
                times.UserCountry.Equals(user.Country) &&
                times.UserState == user.State
                )
                .ToListAsync();
        }


        public async Task<List<TimeTableModel>> GetRestaurantsForServingPeople_ForUser(UserModel user)
        {
            return await _databaseContext.TimeTables
                .Include(times => times.Restuarant)
                .Where(
                times => times.Restuarant.ServeCustomersInPlace &&
                times.UserCity.Equals(user.City) &&
                times.UserCountry.Equals(user.Country) &&
                times.UserState == user.State
                )
                .ToListAsync();
        }


        public async Task<bool> CheckForReservation(ReservationModel reservation)
        {
            return await _databaseContext.Restaurants.FirstOrDefaultAsync(
                restaurant => restaurant.ServeCustomersInPlace &&

                restaurant.ReservationMinAdults <= reservation.Amount_Of_Adults &&
                restaurant.ReservationMaxAdults >= reservation.Amount_Of_Adults
                &&
                restaurant.ReservationMinChildren <= reservation.Amount_Of_Children &&
                restaurant.ReservationMaxChildren >= reservation.Amount_Of_Children
                ) != null;
        }


        // The user's address is the address of the restaurant, they work in.
        // This only applies to people, who take care of reservations and cooks,
        // because they need to be present in the restaurant.
        public async Task<RestaurantModel?> GetRestaurantForStaff(UserModel user)
        {
            return await _databaseContext.Restaurants.FirstOrDefaultAsync(
                restaurant => CheckUserWorkingInThatRestaurat(user, restaurant)
                );
        }


        public bool CheckUserWorkingInThatRestaurat(UserModel user, RestaurantModel restaurant) {
            return restaurant.ServeCustomersInPlace &&
                restaurant.Address.Equals(user.Address) &&
                restaurant.City.Equals(user.City) &&
                restaurant.Country.Equals(user.Country) &&
                restaurant.State == user.State;
        }


        public int? GetRestaurantIdFromCookieHeader(HttpContext context)
        {
            context.Request.Cookies.TryGetValue("restaurant_id", out string? restaurant_id_str);
            if (!int.TryParse(restaurant_id_str, out int restaurant_id))
            {
                return null;
            }

            return restaurant_id;
        }
    }
}
