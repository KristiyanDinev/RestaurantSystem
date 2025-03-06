using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;

namespace ITStepFinalProject.Database.Handlers
{
    public class ServiceDatabaseHandler
    {
        private static readonly string table = "Services";
        private static readonly string tableRoles = "Roles";

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
               .ConditionKeyword("WHERE")
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
               .ConditionKeyword("WHERE")
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
               .ConditionKeyword("WHERE")
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
                .ConditionKeyword("ON")
                .BuildCondition(tableRoles + ".Role", '"'+table+ "\".\"Role\"")
                .ConditionKeyword("WHERE")
                .BuildCondition("UserId", user.Id, "=", "AND ")
                .BuildCondition("Service", ValueHandler.Strings(service, true))
                .ToString(), new JoinedServiceAndRoleModel());

            return res.Models.Count != 0;
        }
    }
}
