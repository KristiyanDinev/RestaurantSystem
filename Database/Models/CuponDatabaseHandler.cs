using ITStepFinalProject.Models;
using Npgsql;

namespace ITStepFinalProject.Database.Models
{
    public class CuponDatabaseHandler
    {
        public static async void DeleteCupon(string cuponCode)
        {
            string cuponSql = $"DELETE FROM Cupons WHERE CuponCode = {_handleStrings(cuponCode)};";

            var cuponCMD = await DatabaseCommandBuilder.BuildCommand(cuponSql, null);
            int num = await cuponCMD.ExecuteNonQueryAsync();

            cuponCMD.Connection?.Close();
            cuponCMD.Dispose();

            if (num <= 0)
            {
                throw new Exception("Can't delete cupon");
            }
        }

        public static async Task<CuponModel> GetCuponByCode(string cuponCode)
        {
            string sql = $"SELECT * FROM Cupons WHERE CuponCode = {_handleStrings(cuponCode)}";

            var cmd = await DatabaseCommandBuilder.BuildCommand(sql, null);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            CuponModel model = new CuponModel();
            while (await reader.ReadAsync())
            {
                model = ConvertToCupon(reader);
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();

            return model;
        }
    }
}
