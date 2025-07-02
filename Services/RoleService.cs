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

        public async Task<bool> GrantServiceAccessToRoleAsync(string servicePath, string roleName)
        {
            // Check if service and role exist
            bool serviceExists = await _databaseContext.Services.AnyAsync(s => s.Path.Equals(servicePath));
            bool roleExists = await _databaseContext.Roles.AnyAsync(r => r.Name.Equals(roleName));

            if (!serviceExists || !roleExists) {
                return false;
            }

            // Check if permission already exists
            if (await _databaseContext.RolePermissions.AnyAsync(
                rp => rp.ServicePath.Equals(servicePath) && rp.RoleName.Equals(roleName))) {
                return true;
            }

            // Create new permission
            _databaseContext.RolePermissions.Add(new RolePermissionModel
            {
                ServicePath = servicePath,
                RoleName = roleName
            });

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RevokeServiceAccessFromRoleAsync(string servicePath, string roleName)
        {
            RolePermissionModel? permission = await _databaseContext.RolePermissions.FindAsync(roleName, servicePath);

            if (permission == null) {
                return false;
            }

            _databaseContext.RolePermissions.Remove(permission);
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

        // list of role names for user
        public async Task<List<RoleEnum>> GetUserRolesAsync(int userId)
        {
            List<string> roleNames = await _databaseContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleName)
                .ToListAsync();

            return roleNames
                .Select(roleName => Enum.TryParse<RoleEnum>(roleName, true, out RoleEnum roleEnum) ? 
                    roleEnum : (RoleEnum?)null)
                .Where(e => e.HasValue)
                .Select(e => e.Value)
                .ToList();
        }

        // List of names of the services
        public async Task<List<string>> GetRoleServicesAsync(string roleName)
        {
            return await _databaseContext.RolePermissions
                .Where(rp => rp.RoleName.Equals(roleName))
                .Select(rp => rp.ServicePath)
                .ToListAsync();
        }

        // List of service names, whcih the user can access
        public async Task<List<string>> GetUserAccessibleServicesAsync(int userId)
        {
            // Get user's roles
            List<string> userRoles = await _databaseContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleName)
                .ToListAsync();

            if (userRoles.Count == 0)
            {
                return new List<string>();
            }

            // Get services accessible by any of these roles
            return await _databaseContext.RolePermissions
                .Where(rp => userRoles.Contains(rp.RoleName))
                .Select(rp => rp.ServicePath)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<UserModel>> GetUsersWithAccessToServicesAsync(List<string> services)
        {
            return await _databaseContext.Users
                .Include(user => user.Roles)
                .Where(user => 
                user.Roles.Any(userRole =>
                userRole.Role.RolePermissions.Any(rolePermission =>
                services.Contains(rolePermission.ServicePath))))
                .ToListAsync();
        }
    }
}
