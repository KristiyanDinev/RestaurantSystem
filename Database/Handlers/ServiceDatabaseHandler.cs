using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;

namespace ITStepFinalProject.Database.Handlers
{
    public class ServiceDatabaseHandler
    {
        private static readonly string table = "Services";
        private static readonly string tableRoles = "Roles";

        public void AddRolesToServices(List<ServiceModel> serviceModels)
        {
            DatabaseManager._ExecuteNonQuery(
                new SqlBuilder()
                .Insert(table, serviceModels.Cast<object>().ToList()).ToString()
                , true);
        }

        public void RemoveService(string service)
        {
            DatabaseManager._ExecuteNonQuery(
               new SqlBuilder()
               .Delete(table)
               .ConditionKeyword("WHERE")
               .BuildCondition("Service", ValueHandler.Strings(service))
               .ToString(), false);
        }

        public void RemoveRolesFromService(List<ServiceModel> serviceModels)
        {
            List<string> Roles = new List<string>();
            List<string> Services = new List<string>();
            foreach (ServiceModel serviceModel in serviceModels)
            {
                Roles.Add(ValueHandler.Strings(serviceModel.Role));
                Services.Add(ValueHandler.Strings(serviceModel.Service));
            }

            DatabaseManager._ExecuteNonQuery(
               new SqlBuilder()
               .Delete(table)
               .ConditionKeyword("WHERE")
               .BuildCondition("Service", '(' + string.Join(", ", Services) + ')', "IN", "AND ")
               .BuildCondition("Role", '(' + string.Join(", ", Roles) + ')', "IN")
               .ToString(), true);
        }

        public void RemoveRolesFromUser(List<UserRoleModel> userRoleModels)
        {
            List<int> Ids = new List<int>();
            List<string> Roles = new List<string>();
            foreach (UserRoleModel userRoleModel in userRoleModels)
            {
                Ids.Add(userRoleModel.UserId);
                Roles.Add(ValueHandler.Strings(userRoleModel.Role));
            }

            DatabaseManager._ExecuteNonQuery(
               new SqlBuilder()
               .Delete(tableRoles)
               .ConditionKeyword("WHERE")
               .BuildCondition("UserId", '('+string.Join(", ", Ids) + ')', "IN", "AND ")
               .BuildCondition("Role", '(' + string.Join(", ", Roles) + ')', "IN")
               .ToString(), true);
        }

        public void AddRolesToUser(List<UserRoleModel> userRoleModels)
        {
            DatabaseManager._ExecuteNonQuery(
                new SqlBuilder()
                .Insert(tableRoles, userRoleModels.Cast<object>().ToList()).ToString()
                , true);
        }

        public async Task<bool> DoesUserHaveRolesToAccessService(UserModel user, string service)
        {
            List<object> res = await DatabaseManager._ExecuteQuery(
                new SqlBuilder()
                .Select("*", table)
                .Join(tableRoles, "INNER")
                .ConditionKeyword("ON")
                .BuildCondition(tableRoles + ".Role", '"'+table+ "\".\"Role\"")
                .ConditionKeyword("WHERE")
                .BuildCondition("UserId", user.Id, "=", "AND ")
                .BuildCondition("Service", ValueHandler.Strings(service, true))
                .ToString(), new JoinedServiceAndRoleModel(), true);

            return res.Count != 0;
        }
    }
}
