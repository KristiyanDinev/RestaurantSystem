using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
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

        public async Task<UserModel?> GetStaffUser(int id)
        {
            return await _databaseContext.Users
                .Include(user => user.Restaurant)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<UserModel?> RegisterUser(RegisterFormModel registerFormModel)
        {
            UserModel user = new UserModel
            {
                PhoneNumber = registerFormModel.PhoneNumber,
                Address = registerFormModel.Address,
                City = registerFormModel.City,
                State = registerFormModel.State,
                Country = registerFormModel.Country,
                Email = registerFormModel.Email,
                Image = registerFormModel.Image,
                Name = registerFormModel.Name,
                Password = Convert.ToBase64String(EncryptionUtility.HashIt(registerFormModel.Password)),
                Notes = registerFormModel.Notes,
                PostalCode = registerFormModel.PostalCode
            };

            await _databaseContext.Users.AddAsync(user);

            return await _databaseContext.SaveChangesAsync() > 0 ? user : null;
        }

        public async Task<UserModel?> LoginUser(string email, string no_hash_password)
        {
            string hash_password = Convert.ToBase64String(EncryptionUtility.HashIt(no_hash_password));
            return await _databaseContext.Users.FirstOrDefaultAsync(
                user => user.Email.Equals(email) && 
                user.Password.Equals(hash_password));
        }


        public async Task<bool> UpdateUser(UserModel user, 
            ProfileUpdateFormModel profileUpdateFormModel)
        {
            user.State = profileUpdateFormModel.State;
            user.Address = profileUpdateFormModel.Address;
            user.City = profileUpdateFormModel.City;
            user.Country = profileUpdateFormModel.Country;
            user.PhoneNumber = profileUpdateFormModel.PhoneNumber;
            user.Image = profileUpdateFormModel.Image;
            user.Name = profileUpdateFormModel.Name;
            user.Notes = profileUpdateFormModel.Notes;

            return await _databaseContext.SaveChangesAsync() > 0;
        }


        public async Task<bool> DeleteUser(int userId)
        {
            UserModel? user = await _databaseContext.Users.FirstOrDefaultAsync(
                user => user.Id == userId);

            if (user == null)
            {
                return false;
            }

            _databaseContext.Users.Remove(user);

            return await _databaseContext.SaveChangesAsync() > 0;
        }


        public async Task<RestaurantModel?> GetRestaurantWhereUserWorksIn(UserModel user)
        {
            return await _databaseContext.Restaurants
                .Where(restaurant => restaurant.Id == user.RestaurantId)
                .FirstOrDefaultAsync();
        }
    }
}
