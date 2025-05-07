using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Utilities;
namespace RestaurantSystem.Services
{
    public class UserService
    {

        private DatabaseContext _databaseContext;

        public UserService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<UserModel?> GetUser(int id)
        {
            return await _databaseContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<UserModel> RegisterUser(string name, string email, string no_hash_password, 
            string address, string city, string? state, string country, 
            string? notes, string? phoneNumber, string? image)
        {
            UserModel user = new UserModel
            {
                PhoneNumber = phoneNumber,
                Address = address,
                City = city,
                State = state,
                Country = country,
                Email = email,
                Image = image,
                Name = name,
                Password = Convert.ToBase64String(EncryptionUtility.HashIt(no_hash_password)),
                Notes = notes
            };

            await _databaseContext.Users.AddAsync(user);

            await _databaseContext.SaveChangesAsync();

            return user;
        }

        public async Task<UserModel?> LoginUser(string email, string hash_password)
        {
            return await _databaseContext.Users.FirstOrDefaultAsync(
                user => user.Email.Equals(email) && user.Password.Equals(hash_password));
        }

        public async Task UpdateUser(UserModel new_user)
        {
            UserModel? user = await GetUser(new_user.Id);
            if (user == null) {
                return;
            }

            user.State = new_user.State;
            user.Address = new_user.Address;
            user.City = new_user.City;
            user.Country = new_user.Country;
            user.Password = new_user.Password;
            user.Roles = new_user.Roles;
            user.PhoneNumber = new_user.PhoneNumber;
            user.Image = new_user.Image;
            user.Name = new_user.Name;
            user.Notes = new_user.Notes;

            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteUser(int userId)
        {
            UserModel? user = await _databaseContext.Users.FirstOrDefaultAsync(
                user => user.Id == userId);

            if (user == null)
            {
                return;
            }

            _databaseContext.Users.Remove(user);

            await _databaseContext.SaveChangesAsync();
        }


        public async Task<RestaurantModel?> GetRestaurantWhereUserWorksIn(UserModel user)
        {
            return await _databaseContext.Restaurants
                .Include(restaurant => restaurant.Employees)
                .Where(restaurant => restaurant.Id == restaurantId)
                .Select(restaurant => restaurant.Employees)
                .FirstOrDefaultAsync();
        }
    }
}
