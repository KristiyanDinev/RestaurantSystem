
namespace RestaurantSystem.Extentions
{
    public static class SessionExtention
    {

        public static void UseSessions(this IHostBuilder host)
        {
            host.ConfigureServices(
                s => s.AddSession(options => {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                })
            );
        }
    }
}
