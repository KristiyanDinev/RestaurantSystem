using ITStepFinalProject.Models;
using ITStepFinalProject.Database.Utils;

namespace ITStepFinalProject.Database.Handlers
{
    public class CuponDatabaseHandler
    {
        public async void DeleteCupon(string cuponCode)
        {
            List<string> res = new List<string>();
            res.Add("CuponCode = "+ValueHandler.Strings(cuponCode));

            DatabaseManager._ExecuteNonQuery(new SqlBuilder().Delete("Cupons")
                .Where_Set_On_Having("WHERE", res).ToString());
        }

        public async Task<CuponModel> GetCuponByCode(string cuponCode)
        {
            List<string> res = new List<string>();
            res.Add("CuponCode = " + ValueHandler.Strings(cuponCode));

            List<object> objects = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("*", "Cupons")
                .Where_Set_On_Having("WHERE", res).ToString(), new CuponModel(), true);

            return (CuponModel)objects[0];
        }
    }
}
