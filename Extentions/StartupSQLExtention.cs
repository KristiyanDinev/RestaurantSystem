using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using Serilog;
using System.Data;

namespace RestaurantSystem.Extentions
{

    public static class StartupSQLExtention
    {

        public async static void UseStartupSQL(this IApplicationBuilder builder, WebApplication app)
        {
            Log.Information("Start up SQL executing...");
            string? sqlFile = app.Configuration.GetValue<string>("StartUpSQLFile");
            if (sqlFile != null && File.Exists(sqlFile))
            {
                using IServiceScope scope = app.Services.CreateScope();
                DatabaseContext dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                using var connection = dbContext.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }


                using var command = connection.CreateCommand();
                command.CommandText = File.ReadAllText(sqlFile);
                command.CommandTimeout = 300000;
                command.ExecuteNonQuery();
            }

            Log.Information("SQL executed");
        }
    }
}
