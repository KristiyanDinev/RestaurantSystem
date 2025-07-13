using RestaurantSystem.Extentions;
using RestaurantSystem.Middlewares;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;
using Serilog;

namespace RestaurantSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            // Replace default logging with Serilog
            builder.Host.UseSerilog((context, loggerConfig) =>
            {
                loggerConfig.ReadFrom.Configuration(context.Configuration);
            });

            Log.Information("Current Working Directory: " + Directory.GetCurrentDirectory());
            Log.Information("Configuring");

            builder.Host.UseSessions();
            builder.Host.UseControllersWithViews();

            string uri = builder.Configuration.GetValue<string>("Uri")
                    ?? "http://127.0.0.1:7278";

            builder.WebHost.UseUrls([uri]);

            builder.Host.UseDatabaseContext(builder.Configuration);

            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<DishService>();
            builder.Services.AddScoped<OrderedDishesService>();
            builder.Services.AddScoped<RestaurantService>();
            builder.Services.AddScoped<CuponService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<RoleService>();
            builder.Services.AddScoped<ReservationService>();
            builder.Services.AddScoped<LocationService>();
            builder.Services.AddScoped<AddressService>();
            builder.Services.AddScoped<DeliveryService>();

            builder.Services.AddScoped<EncryptionUtility>(_ =>
                new EncryptionUtility(builder.Configuration.GetValue<string>("Encryption_Key") ??
                "D471E0624EA5A7FFFABAA918E87"));

            builder.Services.AddScoped<JWTUtility>(_ =>
                new JWTUtility(builder.Configuration.GetValue<string>("JWT_Key") ?? 
                "234w13543ewf53erdfa"));

            builder.Services.AddScoped<UserUtility>();
            builder.Services.AddScoped<WebSocketService>();
            builder.Services.AddSingleton<WebSocketUtility>();
            builder.Host.UseRateLimits();

            WebApplication app = builder.Build();

            string ipPort = uri.Split("/").Last();
            // Security headers middleware
            app.Use(async (context, next) =>
            {
                // Content Security Policy
                context.Response.Headers.Add("Content-Security-Policy",
                    "default-src 'self';" +
                    "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://stackpath.bootstrapcdn.com;" +
                    "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://stackpath.bootstrapcdn.com;" +
                    "font-src 'self' https://cdn.jsdelivr.net;" +
                    "img-src 'self' data: https:;" +
                    "connect-src 'self' ws://localhost:* wss://localhost:* ws://127.0.0.1:* wss://127.0.0.1:* wss://" +
                    ipPort + " ws://"+ ipPort + " wss://"+ ipPort+";");

                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                await next();
            });

            app.UseWebSockets(uri);
            app.UseSession();
            app.UseRateLimiter();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthenticationMiddleware();
            app.UseWebSocketMiddleware();
            app.UseLoggingMiddleware();
            app.MapControllers();
            app.UseStartupSQL(app);

            app.Run();
        }
    }
}
