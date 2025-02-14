using ITStepFinalProject.Controllers;
using ITStepFinalProject.Database;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Cryptography;
using System.Threading.RateLimiting;

namespace ITStepFinalProject
{
    public class Program
    {
        public static HashAlgorithm hashing;

        public static void Main(string[] args)
        {
            hashing = SHA256.Create();
            

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
            DatabaseConnection._connectionString = 
                builder.Configuration.GetValue<string>("ConnectionString") ?? "";

            DatabaseManager.Setup();

            builder.Services.AddScoped<DatabaseManager>();

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



            app.UseSession();
            app.UseStaticFiles();
            app.UseRateLimiter();


            app.Use(async (HttpContext context, RequestDelegate next) => {

                ISession session = context.Session;
                await session.LoadAsync();
                if (!session.IsAvailable) {
                    return;
                }

                if (Utils.Utils.IsDateExpired(session,
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
