using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using RestaurantSystem.Database;
using RestaurantSystem.Middlewares;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;
using System.Data;
using System.Data.Common;
using System.Threading.RateLimiting;

namespace RestaurantSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Current Working Directory: "+ Directory.GetCurrentDirectory());
            Console.WriteLine("Configuring");
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddControllersWithViews();

            string uri = builder.Configuration.GetValue<string>("Uri")
                    ?? "http://127.0.0.1:7278";

            builder.WebHost.UseUrls([uri]);
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
            builder.Services.AddScoped<LocationService>();
            builder.Services.AddScoped<AddressService>();

            builder.Services.AddScoped<EncryptionUtility>(_ =>
                new EncryptionUtility(builder.Configuration.GetValue<string>("Encryption_Key") ??
                "D471E0624EA5A7FFFABAA918E87"));

            builder.Services.AddScoped<JWTUtility>(_ =>
                new JWTUtility(builder.Configuration.GetValue<string>("JWT_Key") ?? 
                "234w13543ewf53erdfa"));

            builder.Services.AddScoped<UserUtility>();
            builder.Services.AddScoped<WebSocketService>();
            builder.Services.AddSingleton<WebSocketUtility>();

            builder.Services.AddRateLimiter(_ => _
                .AddFixedWindowLimiter(policyName: "fixed", options => {
                    options.PermitLimit = 1;
                    options.Window = TimeSpan.FromSeconds(1);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 2;
                })
            );

            WebApplication app = builder.Build();
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2),
                AllowedOrigins = { uri }
            });

            app.UseSession();
            app.UseRateLimiter();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthenticationMiddleware();
            app.UseWebSocketMiddleware();
            app.UseLoggingMiddleware();
            app.MapControllers();

            Console.WriteLine("Start up SQL executing...");
            string? sqlFile = builder.Configuration.GetValue<string>("StartUpSQLFile");
            if (sqlFile != null && File.Exists(sqlFile)) {
                using IServiceScope scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
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

            Console.WriteLine("SQL executed");

            app.Run();
        }
    }
}
