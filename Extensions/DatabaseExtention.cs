using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;

namespace RestaurantSystem.Extentions
{
    public static class DatabaseExtention
    {

        public static void UseDatabaseContext(this IHostBuilder host, IConfigurationManager configuraton)
        {
            host.ConfigureServices(
                s => s.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseNpgsql(configuraton.GetValue<string>("ConnectionString"));
                })
            );
        }
    }
}
