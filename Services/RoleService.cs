using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Enums;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class RoleService
    {

        private DatabaseContext _databaseContext;

        public RoleService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> AssignRoleToUserAsync(long userId, RoleEnum roleName)
        {
            // Check if assignment already exists
            if (await _databaseContext.UserRoles.AnyAsync(
                ur => ur.UserId == userId && ur.RoleName.Equals(roleName.ToString())))
            {
                return true;
            };

            // Create new assignment
            await _databaseContext.UserRoles.AddAsync(new UserRoleModel
            {
                UserId = userId,
                RoleName = roleName.ToString()
            });

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRoleFromUserAsync(long userId, RoleEnum roleName)
        {
            UserRoleModel? userRole = await _databaseContext.UserRoles
                .FirstOrDefaultAsync(u => 
                    u.UserId == userId && 
                    u.RoleName.Equals(roleName.ToString()));
            if (userRole == null) {
                return false;
            }
            _databaseContext.UserRoles.Remove(userRole);
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> CanUserAccessServiceAsync(int userId, string servicePath)
        {
            // Get all roles for the user
            List<string> userRoles = await _databaseContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleName)
                .ToListAsync();

            if (userRoles.Count == 0) {
                return false;
            }

            // Check if any of the user's roles has access to the service
            return await _databaseContext.RolePermissions
                .AnyAsync(rp => servicePath.StartsWith(rp.ServicePath) && 
                    userRoles.Contains(rp.RoleName));
        }
    }
}
