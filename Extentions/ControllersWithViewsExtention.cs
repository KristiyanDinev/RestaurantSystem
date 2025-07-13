namespace RestaurantSystem.Extentions
{
    public static class ControllersWithViewsExtention
    {

        public static void UseControllersWithViews(this IHostBuilder host)
        {
            host.ConfigureServices(
                s => s.AddControllersWithViews()
                .AddCookieTempDataProvider(options =>
                {
                    options.Cookie.Name = "restaurant_temp_data";
                })
            );
        }
    }
}
