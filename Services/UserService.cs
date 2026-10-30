using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<UserModel> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(DatabaseContext databaseContext, UserManager<UserModel> userManager,
            ILogger<UserService> logger)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<UserModel?> GetUserAsync(long id)
        {
            return await _databaseContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<UserModel?> GetUserByEmailAsync(string Email)
        {
            return await _userManager.FindByEmailAsync(Email.Trim());
        }

        public async Task<UserModel?> GetUserWithRolesAsync(long id)
        {
            UserModel? user = await GetUserAsync(id);
            return user == null ? null : await WithRolesAsync(user);
        }

        public async Task<UserModel?> GetStaffUserAsync(long id)
        {
            return await _databaseContext.Users
                .Include(user => user.Restaurant)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<UserModel?> GetStaffUserWithRolesAsync(long id)
        {
            UserModel? user = await _databaseContext.Users
                .Include(user => user.Restaurant)
                .FirstOrDefaultAsync(user => user.Id == id);
            return user == null ? null : await WithRolesAsync(user);
        }

        public async Task<(UserModel? User, IdentityResult Result)> RegisterUserAsync(
            RegisterFormModel registerFormModel)
        {
            UserModel user = new UserModel
            {
                Name = registerFormModel.Name.Trim(),
                UserName = registerFormModel.Email.Trim(),
                Email = registerFormModel.Email.Trim(),
                Image = await Utility.UploadUserImageAsync(registerFormModel.Image),
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerFormModel.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Identity user creation failed for {Email}: {Errors}",
                    user.Email, string.Join("; ",
                        result.Errors.Select(error => $"{error.Code}: {error.Description}")));
            }
            return (result.Succeeded ? user : null, result);
        }

        public async Task<UserModel?> LoginUserAsync(string email, string no_hash_password)
        {
            UserModel? user = await _userManager.FindByEmailAsync(email.Trim());
            return user?.PasswordHash != null &&
                await _userManager.CheckPasswordAsync(user, no_hash_password)
                ? user : null;
        }

        public async Task<bool> UpdateLastLoginDate(UserModel user)
        {
            user.LastTimeLoggedIn = DateOnly.FromDateTime(DateTime.Now);
            return await _userManager.UpdateAsync(user) is { Succeeded: true };
        }
        
        public async Task<bool> UpdateUserPasswordAsync(string Email, string no_hash_password)
        {
            UserModel? user = await GetUserByEmailAsync(Email);
            if (user == null)
            {
                return false;
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return (await _userManager.ResetPasswordAsync(user, token, no_hash_password)).Succeeded;
        }

        public async Task<bool> UpdateUserAsync(UserModel user,
            ProfileUpdateFormModel profileUpdateForm)
        {
            if (profileUpdateForm.DeleteImage)
            {
                Utility.DeleteImage(user.Image);
                user.Image = null;

            }
            else
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
            user.UserName = profileUpdateForm.Email;
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Identity profile update failed for user {UserId}: {Errors}",
                    user.Id, string.Join("; ",
                        result.Errors.Select(error => $"{error.Code}: {error.Description}")));
            }
            return result.Succeeded;
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

            return (await _userManager.DeleteAsync(user)).Succeeded;
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
            return (await _userManager.UpdateAsync(user)).Succeeded;
        }

        public async Task<bool> RemoveEmployeeFromRestaurantAsync(UserModel user)
        {
            user.RestaurantId = null;
            return (await _userManager.UpdateAsync(user)).Succeeded;
        }

        public async Task<List<UserModel>> GetRestaurantEmployeesAsync(
            int restaurantId, int page)
        {
            List<UserModel>? employees = await Utility
                .GetPageAsync<UserModel>(_databaseContext.Users
                .Where(user => user.RestaurantId == restaurantId)
                .OrderBy(user => user.Name)
                .AsQueryable(), page)
                .ToListAsync();
            foreach (UserModel employee in employees ?? [])
            {
                await WithRolesAsync(employee);
            }
            return employees ?? [];
        }

        private async Task<UserModel> WithRolesAsync(UserModel user)
        {
            user.Roles = (await _userManager.GetRolesAsync(user))
                .Select(roleName => new RoleModel
                {
                    Name = roleName
                })
                .ToList();
            return user;
        }
    }
}
