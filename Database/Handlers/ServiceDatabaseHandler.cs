using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Database.Handlers
{
    public class ServiceDatabaseHandler
    {

        public DatabaseManager _databaseManager;

        public ServiceDatabaseHandler(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        // Throws an exception for errors.
        public async Task AddRoleToUser(int userId, string role)
        {
            UserModel user = await _databaseManager.Users
                .Include(user => user.Roles)
                .FirstOrDefaultAsync(user => user.Id == userId) ?? 
                throw new Exception("No such user");

            user.Roles ??= new List<UserRoleModel>();

            if (user.Roles.Any(ur => ur.Role.Equals(role)))
            {
                throw new Exception("No such role");
            }

            UserRoleModel roleModel = new UserRoleModel();
            roleModel.UserModelId = userId;
            roleModel.Role = role;

            user.Roles.Add(roleModel);

            await _databaseManager.SaveChangesAsync();
        }

        // Throws an exception for errors.
        public async Task RemoveRoleFromUser(int userId, string role)
        {
            UserModel user = await _databaseManager.Users
                .Include(user => user.Roles)
                .FirstOrDefaultAsync(user => user.Id == userId) ??
                throw new Exception("No such user");

            user.Roles ??= new List<UserRoleModel>();

            UserRoleModel userRole = user.Roles.FirstOrDefault(u => u.Role.Equals(role)) ??
                throw new Exception("No such role");

            user.Roles.Remove(userRole);

            await _databaseManager.SaveChangesAsync();
        }

        public async Task GrantRoleAccessToService(string role, string new_service)
        {
            // user -> role -> service

            ServiceModel serviceModel = await _databaseManager.Services
                .Include(s => s.AllowedRoles)
        .       FirstOrDefaultAsync(s => s.Service == servicePath);

            if (service == null)
            {
                service = new ServiceModel
                {
                    Service = servicePath,
                    AllowedRoles = new List<UserRoleModel>()
                };
                _context.Set<ServiceModel>().Add(service);
            }


            UserRoleModel roleModel = await _databaseManager.UserRoles
                .Include(role => role.Services)
                .FirstOrDefaultAsync(s => s.Role.Equals(role)) ?? 
                throw new Exception("No such role");

            ServiceModel serviceModel = new ServiceModel();
            serviceModel.Service = new_service;
            serviceModel.AllowedRoles

            roleModel.Services.Add();
        }

        public async Task<UserRoleModel> CheckUserAccess(int userId, string service)
        {
            UserModel user = await _databaseManager.Users
                .Include(user => user.Roles)
                .FirstOrDefaultAsync(user => user.Id == userId) ??
                throw new Exception("No such user");

            return user.Roles.FirstOrDefault(r => r.Services.Equals(service)) ?? 
                throw new Exception("Can't access the resource");
        }
    }
}
