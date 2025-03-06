using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;

namespace ITStepFinalProject.Database.Handlers
{
    public class CuponDatabaseHandler
    {
        private static readonly string table = "Cupons";
        public async void DeleteCupon(string cuponCode)
        {
            await DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Delete(table)
                .ConditionKeyword("WHERE")
                .BuildCondition("CuponCode", ValueHandler.Strings(cuponCode)).ToString());
        }

        public async Task<CuponModel?> GetCuponByCode(string cuponCode)
        {
            ResultSqlQuery res = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("*", table)
                .ConditionKeyword("WHERE")
                .BuildCondition("CuponCode", ValueHandler.Strings(cuponCode))
                .ToString(), new CuponModel());

            return res.Models.Count == 0 ? null : (CuponModel)res.Models[0];
        }
    }
}
