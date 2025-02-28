using ITStepFinalProject.Controllers;
using ITStepFinalProject.Database;
using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using ITStepFinalProject.Services;
using ITStepFinalProject.Utils.Controller;
using ITStepFinalProject.Utils.Utils;
using ITStepFinalProject.Utils.Web;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace ITStepFinalProject
{
    public class Program
    {
        public static List<RestorantAddressModel> resturantAddresses;
        public static string currentDir;
        public static void Main(string[] args)
        {
            resturantAddresses = new List<RestorantAddressModel>();
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
            });*/

            string uri = builder.Configuration.GetValue<string>("Uri")
                    ?? "http://127.0.0.1:7278";

            builder.WebHost.UseUrls([uri]);

            // postgresql 17 5432
            // postgresql 16 5433
            DatabaseManager._connectionString = 
                builder.Configuration.GetValue<string>("ConnectionString") ?? "";

            string key = builder.Configuration.GetValue<string>("Encryption_Key") ?? "D471E0624EA5A7FFFABAA918E87";

            DatabaseManager.Setup();

            EncryptionHandler encryptionHandler = new EncryptionHandler(key);
            JWTHandler jwtHandler = new JWTHandler(key);

            builder.Services.AddScoped<UserDatabaseHandler>();
            builder.Services.AddScoped<DishDatabaseHandler>();
            builder.Services.AddScoped<CuponDatabaseHandler>();
            builder.Services.AddScoped<OrderDatabaseHandler>();

            builder.Services.AddSingleton<WebUtils>(new WebUtils(
                new Dictionary<string, List<string>>{
                       {"User", ["{{UserBar}}", "{{Profile}}"]},
                       {"Dish", ["{{DishDisplay}}", "{{DishCart}}", "{{WholeDish}}"]},
                       {"Order", ["{{OrderDisplay}}"]},
                       {"Restorant", ["{{RestorantAddress}}"] },
                }));

            builder.Services.AddSingleton<ControllerUtils>(
                new ControllerUtils(encryptionHandler, jwtHandler));

            builder.Services.AddSingleton<UserUtils>(
                new UserUtils(encryptionHandler, jwtHandler));

            builder.Services.AddSingleton<WebSocketUtils>();

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

            foreach (string resturantAddress in 
                (builder.Configuration.GetValue<string>("ResturantAddressAvrageDeliverTime")
                ?? "Missing Resturant Address").Split("---"))
            {
                if (resturantAddress.Length == 0)
                {
                    continue;
                }

                try
                {
                    string[] parts = resturantAddress.Split('|');

                    string[] userAddressParts = parts[0].Split(';');

                    RestorantAddressModel resturant = new RestorantAddressModel();
                    resturant.AvrageTime = parts[1];
                    resturant.UserAddress = userAddressParts[0];
                    resturant.UserCity = userAddressParts[1];
                    resturant.UserState = userAddressParts[2];
                    resturant.UserCountry = userAddressParts[3];

                    string[] restorantAddressParts = parts[2].Split(';');
                    resturant.RestorantAddress = restorantAddressParts[0];
                    resturant.RestorantCity = restorantAddressParts[1];
                    resturant.RestorantState = restorantAddressParts[2];
                    resturant.RestorantCountry = restorantAddressParts[3];

                    resturantAddresses.Add(resturant);

                } catch (Exception)
                {
                    continue;
                }
            }

            var app = builder.Build();

            //app.UseSession();
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2),
            };

            webSocketOptions.AllowedOrigins.Add(uri);

            app.UseWebSockets(webSocketOptions);
            app.UseRateLimiter();
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



            new WebSocketController(app);
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
            Console.WriteLine("The application is shutting down.");
        }
    }
}
