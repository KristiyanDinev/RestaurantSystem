using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class RestaurantService
    {

        private DatabaseContext _databaseContext;
        private readonly string restaurantId = "restaurant_id";

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


        public int? GetRestaurantIdFromCookieHeader(HttpContext context)
        {
            context.Request.Cookies.TryGetValue(restaurantId, out string? restaurant_id_str);
            if (!int.TryParse(restaurant_id_str, out int restaurant_id))
            {
                return null;
            }

            return restaurant_id;
        }


        public async Task<List<UserModel>> GetRestaurantEmployeesByRestaurantId(int restaurantId)
        {
            ICollection<UserModel>? employees = await _databaseContext.Restaurants
                .Include(restaurant => restaurant.Employees)
                .Where(restaurant => restaurant.Id == restaurantId)
                .Select(restaurant => restaurant.Employees)
                .FirstOrDefaultAsync();

            return employees == null ? new List<UserModel>() : employees.ToList();
        }

        public async Task<RestaurantModel?> GetRestaurantById(int? restaurantId)
        {
            return restaurantId == null ? null : await _databaseContext.Restaurants
                .FirstOrDefaultAsync(restaurant => restaurant.Id == restaurantId);
        }
    }
}
