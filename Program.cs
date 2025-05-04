using RestaurantSystem.Controllers;
using RestaurantSystem.Database;
using RestaurantSystem.Database.Handlers;
using RestaurantSystem.Services;
using RestaurantSystem.Utils.Controller;
using RestaurantSystem.Utils.Utils;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace RestaurantSystem
{
    public class Program
    {
        public static string currentDir;
        public static void Main(string[] args)
        {
            currentDir = Directory.GetCurrentDirectory();

            Console.WriteLine("Current Working Directory: "+ currentDir);

            var builder = WebApplication.CreateBuilder(args);

            /*
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".Resturant.Session";
                options.Cookie.IsEssential = true;
                //options.IOTimeout = TimeSpan.FromSeconds(20);
                options.Cookie.HttpOnly = true;
            });

            builder.Services.AddControllersWithViews()
            */

            builder.Services.AddControllersWithViews();

            string uri = builder.Configuration.GetValue<string>("Uri")
                    ?? "http://127.0.0.1:7278";

            builder.WebHost.UseUrls([uri]);

            // postgresql 17 5432

            string encryption_key = builder.Configuration.GetValue<string>("Encryption_Key") ?? "D471E0624EA5A7FFFABAA918E87";
            string jwt_key = builder.Configuration.GetValue<string>("JWT_Key") ?? "234w13543ewf53erdfa";
            
            EncryptionHandler encryptionHandler = new EncryptionHandler(encryption_key);
            JWTHandler jwtHandler = new JWTHandler(jwt_key);

            builder.Services.AddDbContext<DatabaseManager>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetValue<string>("ConnectionString"));
            });
            builder.Services.AddScoped<UserDatabaseHandler>();
            builder.Services.AddScoped<DishDatabaseHandler>();
            builder.Services.AddScoped<OrderedDishesDatabaseHandler>();
            builder.Services.AddScoped<RestaurantDatabaseHandler>();
            builder.Services.AddScoped<CuponDatabaseHandler>();
            builder.Services.AddScoped<OrderDatabaseHandler>();
            builder.Services.AddScoped<ServiceDatabaseHandler>();
            builder.Services.AddScoped<ReservationDatabaseHandler>();

            builder.Services.AddSingleton<ControllerUtils>();

            builder.Services.AddSingleton<UserUtils>();

            builder.Services.AddSingleton<WebSocketHandler>();

            string secretKey = builder.Configuration.GetValue<string>("JWT_SecurityKey")
                    ?? "ugyw89ub9Y9H8OP9j1wsfwedS";

            builder.Services.AddRateLimiter(_ => _
                .AddFixedWindowLimiter(policyName: "fixed", options => {
                    options.PermitLimit = 2;
                    options.Window = TimeSpan.FromSeconds(1);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 1;
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
            

            app.Use(async (HttpContext context, RequestDelegate next) =>
            {
                Console.WriteLine("\n"+context.Request.Method + ": " + context.Request.Path.Value);
                Console.WriteLine("\n----\nCookies:");
                foreach (var cookie in context.Request.Cookies)
                {
                    Console.WriteLine(cookie);
                }
                Console.WriteLine("----\n");



                await next(context);
            });

            app.MapControllers();

            new WebSocketController(app);
            new ReservationsController(app);
            new AdminController(app);
            new UserController(app);
            new DishController(app);
            new ErrorController(app);
            new OrderController(app);
            new CartController(app);

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnApplicationExit);

            app.Run();
        }

        public static void OnApplicationExit(object sender, EventArgs e)
        {
            WebSocketHandler.CloseAllSubscribtions();
            Console.WriteLine("Closed websockets.");
        }
    }
}
