using ITStepFinalProject.Controllers;
using ITStepFinalProject.Database;
using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Cryptography;
using System.Threading.RateLimiting;

namespace ITStepFinalProject
{
    public class Program
    {
        public static HashAlgorithm hashing;
        public static List<RestorantAddressModel> resturantAddresses;
        public static string currentDir;
        public static void Main(string[] args)
        {
            hashing = SHA256.Create();
            resturantAddresses = new List<RestorantAddressModel>();
            currentDir = Directory.GetCurrentDirectory();


            Console.WriteLine("Current Working Directory: "+ currentDir);

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".Resturant.Session";
                options.Cookie.IsEssential = true;
                //options.IOTimeout = TimeSpan.FromSeconds(20);
                options.Cookie.HttpOnly = true;
            });

            string uri = builder.Configuration.GetValue<string>("Uri")
                    ?? "https://127.0.0.1:7278";

            builder.WebHost.UseUrls([uri]);

            // postgresql 17 5432
            // postgresql 16 5433
            DatabaseCommandBuilder._connectionString = 
                builder.Configuration.GetValue<string>("ConnectionString") ?? "";

            WebHelper.commonPlaceholders = new Dictionary<string, List<string>>{
                                                {"User", ["{{UserBar}}", "{{Profile}}"]},
                                                {"Dish", ["{{DishDisplay}}", "{{DishCart}}", "{{WholeDish}}"]},
                                                {"Order", ["{{OrderDisplay}}"]},
                                                {"Restorant", ["{{RestorantAddress}}"] }, 
                };

            DatabaseManager.Setup();

            builder.Services.AddScoped<UserDatabaseHandler>();
            builder.Services.AddScoped<DishDatabaseHandler>();
            builder.Services.AddScoped<CuponDatabaseHandler>();
            builder.Services.AddScoped<OrderDatabaseHandler>();

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

            app.UseSession();
            app.UseRateLimiter();
            app.UseStaticFiles();


            app.Use(async (HttpContext context, RequestDelegate next) => {

                /* something like: /login/login.js
                string? v = context.Request.Path.Value;
                if (v != null && v.Contains('.'))
                {
                    context.Response.Clear();
                    string s = await ControllerUtils.GetFileContent(v);
                    if (!v.EndsWith(".js") && !v.EndsWith(".css") && !v.EndsWith(".html"))
                    {
                        await context.Response.WriteAsync(s);
                        
                    } else
                    {
                        await context.Response.WriteAsync(s);
                    }
                        return;
                }*/
                ISession session = context.Session;
                await session.LoadAsync();
                if (!session.IsAvailable) {
                    return;
                }

                if (ControllerUtils.IsDateExpired(session,
                    "UserId_ExpirationDate")) {
                    context.Response.Redirect(uri + "/login");
                    return;
                }


                await next.Invoke(context);
            });


            new UserController(app);
            new DishController(app);
            new ErrorController(app);
            new OrderController(app);
            new CartController(app);

            app.Run();
        }
    }
}
