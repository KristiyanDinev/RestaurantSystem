using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;

namespace ITStepFinalProject.Database.Handlers
{
    public class CuponDatabaseHandler
    {
        private static readonly string table = "Cupons";
        public async void DeleteCupon(string cuponCode)
        {
            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Delete(table)
                .ConditionKeyword("WHERE")
                .BuildCondition("CuponCode", ValueHandler.Strings(cuponCode)).ToString(), true);
        }

        public async Task<CuponModel?> GetCuponByCode(string cuponCode)
        {
            List<object> objects = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("*", table)
                .ConditionKeyword("WHERE")
                .BuildCondition("CuponCode", ValueHandler.Strings(cuponCode))
                .ToString(), new CuponModel(), true);

            return objects.Count == 0 ? null : (CuponModel)objects[0];
        }
    }
}
