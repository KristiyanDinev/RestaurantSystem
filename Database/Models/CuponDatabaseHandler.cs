using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using ITStepFinalProject.Database.Utils;
using Npgsql;
using System.Linq;

namespace ITStepFinalProject.Database.Models
{
    public class CuponDatabaseHandler
    {
        public async void DeleteCupon(string cuponCode)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("CuponCode", ValueHandler.Strings(cuponCode));

            DatabaseManager._ExecuteNonQuery(new SqlBuilder().Delete("Cupons")
                .Where_Set("WHERE", res).ToString());
        }

        public async Task<CuponModel> GetCuponByCode(string cuponCode)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("CuponCode", ValueHandler.Strings(cuponCode));

            List<object> objects = await DatabaseManager._ExecuteQuery(new SqlBuilder().Select("*", "Cupons")
                .Where_Set("WHERE", res).ToString(), new CuponModel(), true);

            return (CuponModel)objects[0];
        }
    }
}
