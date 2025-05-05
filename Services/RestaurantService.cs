using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class RestaurantService
    {

        private DatabaseContext _databaseManager;

        public RestaurantService(DatabaseContext databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public async Task<List<TimeTableModel>> GetRestaurantsForDelivery_ForUser(UserModel user)
        {
            return await _databaseManager.TimeTables.Where(
                times => times.RestuarantModel.DoDelivery &&
                times.UserCity.Equals(user.City) &&
                times.UserAddress.Equals(user.Address) &&
                times.UserCountry.Equals(user.Country) &&
                times.UserState == user.State
                )
                .ToListAsync();
        }

        public async Task<List<TimeTableModel>> GetRestaurantsForServingPeople_ForUser(UserModel user)
        {
            return await _databaseManager.TimeTables.Where(
                times => times.RestuarantModel.ServeCustomersInPlace &&
                times.UserCity.Equals(user.City) &&
                times.UserAddress.Equals(user.Address) &&
                times.UserCountry.Equals(user.Country) &&
                times.UserState == user.State
                )
                .ToListAsync();
        }

        public async Task<bool> CheckForReservation(ReservationModel reservation)
        {
            return await _databaseManager.Restaurants.FirstOrDefaultAsync(
                restaurant => restaurant.ServeCustomersInPlace &&

                restaurant.ReservationMinAdults <= reservation.Amount_Of_Adults &&
                restaurant.ReservationMaxAdults >= reservation.Amount_Of_Adults
                &&
                restaurant.ReservationMinChildren <= reservation.Amount_Of_Children &&
                restaurant.ReservationMaxChildren >= reservation.Amount_Of_Children
                ) != null;
        }
    }
}
