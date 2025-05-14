using RestaurantSystem.Database;
using RestaurantSystem.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Middlewares;
using RestaurantSystem.Utilities;

namespace RestaurantSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Current Working Directory: "+ Directory.GetCurrentDirectory());

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpLogging(o => { });

            string uri = builder.Configuration.GetValue<string>("Uri")
                    ?? "http://127.0.0.1:7278";

            builder.WebHost.UseUrls([uri]);

            // postgresql 17 5432
            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetValue<string>("ConnectionString"));
            });

            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<DishService>();
            builder.Services.AddScoped<OrderedDishesService>();
            builder.Services.AddScoped<RestaurantService>();
            builder.Services.AddScoped<CuponService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<RoleService>();
            builder.Services.AddScoped<ReservationService>();

            builder.Services.AddScoped<EncryptionUtility>(_ =>
                new EncryptionUtility(builder.Configuration.GetValue<string>("Encryption_Key") ??
                "D471E0624EA5A7FFFABAA918E87"));

            builder.Services.AddScoped<JWTUtility>(_ =>
                new JWTUtility(builder.Configuration.GetValue<string>("JWT_Key") ?? 
                "234w13543ewf53erdfa"));

            builder.Services.AddScoped<Utility>();
            builder.Services.AddScoped<UserUtility>();
            builder.Services.AddScoped<WebSocketService>();

            string secretKey = builder.Configuration.GetValue<string>("JWT_SecurityKey")
                    ?? "ugyw89ub9Y9H8OP9j1wsfwedS";

            builder.Services.AddRateLimiter(_ => _
                .AddFixedWindowLimiter(policyName: "fixed", options => {
                    options.PermitLimit = 1;
                    options.Window = TimeSpan.FromSeconds(1);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 2;
                })
            );

            var app = builder.Build();

            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2),
                AllowedOrigins = { uri }
            });

            app.UseRateLimiter();
            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthenticationMiddleware();
            app.UseWebSocketMiddleware();

            app.UseHttpLogging();
            //app.UseLoggingMiddleware();

            app.MapControllers();
            
            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnApplicationExit);

            app.Run();
        }

        /*
        public static void OnApplicationExit(object sender, EventArgs e)
        {
        }*/
    }
}
