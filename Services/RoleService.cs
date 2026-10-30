using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Enums;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class RoleService
    {

        private DatabaseContext _databaseContext;
        private UserManager<UserModel> _userManager;
        private RoleManager<RoleModel> _roleManager;

        public RoleService(DatabaseContext databaseContext,
            UserManager<UserModel> userManager, RoleManager<RoleModel> roleManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRoleToUserAsync(long userId, RoleEnum roleName)
        {
            UserModel? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            string role = roleName.ToString();
            if (!await _roleManager.RoleExistsAsync(role) &&
                !(await _roleManager.CreateAsync(new RoleModel { Name = role })).Succeeded)
            {
                return false;
            }

            return (await _userManager.AddToRoleAsync(user, role)).Succeeded;
        }

        public async Task<bool> RemoveRoleFromUserAsync(long userId, RoleEnum roleName)
        {
            UserModel? user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null &&
                (await _userManager.RemoveFromRoleAsync(user, roleName.ToString())).Succeeded;
        }

        public async Task<bool> CanUserAccessServiceAsync(long userId, string servicePath)
        {
            UserModel? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Count == 0)
            {
                return false;
            }

            return await _databaseContext.RolePermissions
                .AnyAsync(rp => servicePath.StartsWith(rp.ServicePath) && 
                    userRoles.Contains(rp.RoleName));
        }
    }
}
