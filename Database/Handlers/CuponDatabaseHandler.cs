using ITStepFinalProject.Models;
using ITStepFinalProject.Database.Utils;

namespace ITStepFinalProject.Database.Handlers
{
    public class CuponDatabaseHandler
    {
        private static readonly string table = "Cupons";
        public async void DeleteCupon(string cuponCode)
        {
            List<string> res = new List<string>();
            res.Add("CuponCode = "+ValueHandler.Strings(cuponCode));

            DatabaseManager._ExecuteNonQuery(new SqlBuilder().Delete(table)
                .Where_Set_On_Having("WHERE", res).ToString());
        }

        public async Task<CuponModel> GetCuponByCode(string cuponCode)
        {
            List<string> res = new List<string>();
            res.Add("CuponCode = " + ValueHandler.Strings(cuponCode));

            List<object> objects = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("*", table)
                .Where_Set_On_Having("WHERE", res).ToString(), new CuponModel(), true);

            return (CuponModel)objects[0];
        }
    }
}
