using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Services
{
    public class RestaurantService
    {
        private DatabaseContext _databaseContext;

        public RestaurantService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<RestaurantModel>> GetDeliveryGuy_RestaurantsAsync(AddressModel address)
        {
            return await _databaseContext.Restaurants
                .Where(
                restaurant =>
                restaurant.DoDelivery &&
                restaurant.Country.ToLower().Equals(address.Country.ToLower()) &&
                restaurant.State == address.State &&
                restaurant.City == address.City)
                .ToListAsync();
        }

        public async Task<List<RestaurantModel>> GetAllRestaurantsForUserAsync(UserModel user)
        {
            return await _databaseContext.Restaurants
                .Join(_databaseContext.Addresses,
                      restaurant => new { restaurant.Country, restaurant.State, restaurant.City },
                      address => new { address.Country, address.State, address.City },
                      (restaurant, address) => new { restaurant, address })
                .Where(joined => joined.address.UserId == user.Id)
                .Select(joined => joined.restaurant)
                .Distinct()
                .ToListAsync();
        }


        public async Task<bool> CheckForReservationAsync(ReservationModel reservation)
        {
            return await _databaseContext.Restaurants.FirstOrDefaultAsync(
                restaurant => 
                restaurant.ServeCustomersInPlace &&
                restaurant.Id == reservation.RestaurantId &&

                restaurant.ReservationMinAdults <= reservation.Amount_Of_Adults &&
                restaurant.ReservationMaxAdults >= reservation.Amount_Of_Adults
                &&
                restaurant.ReservationMinChildren <= reservation.Amount_Of_Children &&
                restaurant.ReservationMaxChildren >= reservation.Amount_Of_Children
                ) != null;
        }


        public int? GetRestaurantIdFromCookieHeaderAsync(HttpContext context)
        {
            context.Request.Cookies.TryGetValue(Utility.restaurantId, out string? restaurant_id_str);
            if (!int.TryParse(restaurant_id_str, out int restaurant_id))
            {
                return null;
            }

            return restaurant_id;
        }


        public async Task<List<UserModel>> GetRestaurantEmployeesByRestaurantIdAsync(int restaurantId)
        {
            ICollection<UserModel>? employees = await _databaseContext.Restaurants
                .Include(restaurant => restaurant.Employees)
                .Where(restaurant => restaurant.Id == restaurantId)
                .Select(restaurant => restaurant.Employees)
                .FirstOrDefaultAsync();

            return employees == null ? new List<UserModel>() : employees.ToList();
        }

        public async Task<RestaurantModel?> GetRestaurantByIdAsync(int? restaurantId)
        {
            return restaurantId == null ? null : await _databaseContext.Restaurants
                .FirstOrDefaultAsync(restaurant => restaurant.Id == restaurantId);
        }
    }
}
