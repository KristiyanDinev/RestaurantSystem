using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;

namespace ITStepFinalProject.Database.Handlers
{
    public class ServiceDatabaseHandler
    {
        private static readonly string table = "Services";
        private static readonly string tableRoles = "Roles";
        private static readonly string tableRestorant = "Restorant";

        public async void AddRolesToServices(List<ServiceModel> serviceModels)
        {
            await DatabaseManager._ExecuteNonQuery(
                new SqlBuilder()
                .Insert(table, serviceModels.Cast<object>().ToList()).ToString());
        }

        public async void RemoveService(string service)
        {
            await DatabaseManager._ExecuteNonQuery(
               new SqlBuilder()
               .Delete(table)
               .Where()
               .BuildCondition("Service", ValueHandler.Strings(service))
               .ToString());
        }

        public async void RemoveRolesFromService(List<ServiceModel> serviceModels)
        {
            List<string> Roles = new List<string>();
            List<string> Services = new List<string>();
            foreach (ServiceModel serviceModel in serviceModels)
            {
                Roles.Add(ValueHandler.Strings(serviceModel.Role));
                Services.Add(ValueHandler.Strings(serviceModel.Service));
            }

            await DatabaseManager._ExecuteNonQuery(
               new SqlBuilder()
               .Delete(table)
               .Where()
               .BuildCondition("Service", '(' + string.Join(", ", Services) + ')', "IN", "AND ")
               .BuildCondition("Role", '(' + string.Join(", ", Roles) + ')', "IN")
               .ToString());
        }

        public async void RemoveRolesFromUser(List<UserRoleModel> userRoleModels)
        {
            List<int> Ids = new List<int>();
            List<string> Roles = new List<string>();
            foreach (UserRoleModel userRoleModel in userRoleModels)
            {
                Ids.Add(userRoleModel.UserId);
                Roles.Add(ValueHandler.Strings(userRoleModel.Role));
            }

            await DatabaseManager._ExecuteNonQuery(
               new SqlBuilder()
               .Delete(tableRoles)
               .Where()
               .BuildCondition("UserId", '('+string.Join(", ", Ids) + ')', "IN", "AND ")
               .BuildCondition("Role", '(' + string.Join(", ", Roles) + ')', "IN")
               .ToString());
        }

        public async void AddRolesToUser(List<UserRoleModel> userRoleModels)
        {
            await DatabaseManager._ExecuteNonQuery(
                new SqlBuilder()
                .Insert(tableRoles, userRoleModels.Cast<object>().ToList()).ToString()
                );
        }

        public async Task<bool> DoesUserHaveRolesToAccessService(UserModel user, string service)
        {
            ResultSqlQuery res = await DatabaseManager._ExecuteQuery(
                new SqlBuilder()
                .Select("*", table)
                .Join(tableRoles, "INNER")
                .On()
                .BuildCondition(tableRoles + ".Role", '"'+table+ "\".\"Role\"")
                .Where()
                .BuildCondition("UserId", user.Id, "=", "AND ")
                .BuildCondition("Service", ValueHandler.Strings(service, true))
                .ToString(), new JoinedServiceAndRoleModel());

            return res.Models.Count != 0;
        }

        public async Task<RestorantModel> GetStaffRestorant(UserModel user) {
            // The staff's address should match the restorant they are serving.
            // I don't know if this should appy to delivery staff.


            ResultSqlQuery res = await DatabaseManager._ExecuteQuery(
                new SqlBuilder()
                .Select("*", tableRestorant)
            .Where()
                .BuildCondition("RestorantCity", ValueHandler.Strings(user.City), "=", "AND")
                .BuildCondition("RestorantCountry", ValueHandler.Strings(user.Country), "=", "AND")


                .BuildCondition("RestorantState", ValueHandler.Strings(user.State),
                    string.IsNullOrWhiteSpace(user.State)? "is" : "=", "AND")

                .BuildCondition("RestorantAddress", ValueHandler.Strings(user.Address)).ToString(),
                new RestorantModel());

            return (RestorantModel)res.Models[0];
        }
    }
}
