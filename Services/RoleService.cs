using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class RoleService
    {

        public DatabaseContext _databaseContext;

        public RoleService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> CreateRole(string roleName, string? description = null)
        {
            if (await _databaseContext.Roles.AnyAsync(r => r.Name.Equals(roleName)))
            {
                return false;
            }

            _databaseContext.Roles.Add(new RoleModel
            {
                Name = roleName,
                Description = description
            });

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateService(string path, string? description = null)
        {
            if (await _databaseContext.Services.AnyAsync(s => s.Path.Equals(path)))
            {
                return false;
            }

            _databaseContext.Services.Add(new ServiceModel
            {
                Path = path,
                Description = description
            });

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AssignRoleToUser(int userId, string roleName)
        {
            // Check if user and role exist
            bool userExists = await _databaseContext.Users.AnyAsync(u => u.Id == userId);
            bool roleExists = await _databaseContext.Roles.AnyAsync(r => r.Name.Equals(roleName));

            if (!userExists || !roleExists) {
                return false;
            }

            // Check if assignment already exists
            if (await _databaseContext.UserRoles.AnyAsync(
                ur => ur.UserId == userId && ur.RoleName.Equals(roleName)))
            {
                return true;
            };

            // Create new assignment
            _databaseContext.UserRoles.Add(new UserRoleModel
            {
                UserId = userId,
                RoleName = roleName
            });

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRoleFromUser(int userId, string roleName)
        {
            UserRoleModel? userRole = await _databaseContext.UserRoles.FindAsync(userId, roleName);

            if (userRole == null) {
                return false;
            }

            _databaseContext.UserRoles.Remove(userRole);
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> GrantServiceAccessToRole(string servicePath, string roleName)
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

        public async Task<bool> RevokeServiceAccessFromRole(string servicePath, string roleName)
        {
            RolePermissionModel? permission = await _databaseContext.RolePermissions.FindAsync(roleName, servicePath);

            if (permission == null) {
                return false;
            }

            _databaseContext.RolePermissions.Remove(permission);
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> CanUserAccessService(int userId, string servicePath)
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
                .AnyAsync(rp => rp.ServicePath.Equals(servicePath) && 
                    userRoles.Contains(rp.RoleName));
        }

        // list of role names for user
        public async Task<List<string>> GetUserRoles(int userId)
        {
            return await _databaseContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleName)
                .ToListAsync();
        }

        // List of names of the services
        public async Task<List<string>> GetRoleServices(string roleName)
        {
            return await _databaseContext.RolePermissions
                .Where(rp => rp.RoleName.Equals(roleName))
                .Select(rp => rp.ServicePath)
                .ToListAsync();
        }

        // List of service names, whcih the user can access
        public async Task<List<string>> GetUserAccessibleServices(int userId)
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
    }
}
