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

        public async Task<UserModel?> GetUserAsync(long id)
        {
            return await _databaseContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<UserModel?> GetUserByEmailAsync(string Email)
        {
            return await _databaseContext.Users.FirstOrDefaultAsync(user => user.Email.Equals(Email));
        }

        public async Task<UserModel?> GetUserWithRolesAsync(long id)
        {
            return await _databaseContext.Users
                .Include(user => user.Roles)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<UserModel?> GetStaffUserAsync(long id)
        {
            return await _databaseContext.Users
                .Include(user => user.Restaurant)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<UserModel?> GetStaffUserWithRolesAsync(long id)
        {
            return await _databaseContext.Users
                .Include(user => user.Restaurant)
                .Include(user => user.Roles)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<UserModel?> RegisterUserAsync(RegisterFormModel registerFormModel)
        {
            UserModel user = new UserModel
            {
                Name = registerFormModel.Name,
                Email = registerFormModel.Email,
                Image = await Utility.UploadUserImageAsync(registerFormModel.Image),
                Password = Convert.ToBase64String(EncryptionUtility.HashIt(registerFormModel.Password)),
            };

            await _databaseContext.Users.AddAsync(user);

            return await _databaseContext.SaveChangesAsync() > 0 ? user : null;
        }

        public async Task<UserModel?> LoginUserAsync(string email, string no_hash_password)
        {
            string hash_password = Convert.ToBase64String(EncryptionUtility.HashIt(no_hash_password));
            return await _databaseContext.Users.FirstOrDefaultAsync(
                user => user.Email.Equals(email) && 
                user.Password.Equals(hash_password));
        }

        public async Task<bool> UpdateUserAsync(UserModel user, 
            ProfileUpdateFormModel profileUpdateForm)
        {
            if (profileUpdateForm.DeleteImage)
            {
                Utility.DeleteImage(user.Image);
                user.Image = null;

            } else
            {
                string? img = await Utility.UpdateImageAsync(user.Image,
                    profileUpdateForm.Image);
                if (img != null)
                {
                    user.Image = img;
                }
            }

            user.Name = profileUpdateForm.Name;
            user.Email = profileUpdateForm.Email;

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(long userId)
        {
            UserModel? user = await _databaseContext.Users.FirstOrDefaultAsync(
                user => user.Id == userId);

            if (user == null)
            {
                return false;
            }

            // Delete user image if it exists
            if (user.Image != null)
            {
                Utility.DeleteImage(user.Image);
            }

            _databaseContext.Users.Remove(user);

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<RestaurantModel?> GetRestaurantWhereUserWorksInAsync(UserModel user)
        {
            return await _databaseContext.Restaurants
                .Where(restaurant => restaurant.Id == user.RestaurantId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddEmployeeToRestaurantAsync(UserModel user, int restaurantId)
        {
            user.RestaurantId = restaurantId;
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveEmployeeFromRestaurantAsync(UserModel user)
        {
            user.RestaurantId = null;
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<List<UserModel>> GetRestaurantEmployeesAsync(
            int restaurantId, int page)
        {
            List<UserModel>? employees = await Utility
                .GetPageAsync<UserModel>(_databaseContext.Users
                .Include(user => user.Roles)
                .ThenInclude(role => role.Role)
                .Where(user => user.RestaurantId == restaurantId)
                .AsQueryable(), page)
                .ToListAsync();
            return employees ?? new List<UserModel>();
        }
    }
}
