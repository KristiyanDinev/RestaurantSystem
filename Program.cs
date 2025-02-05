using ITStepFinalProject.Controllers;
using ITStepFinalProject.Database;
using ITStepFinalProject.Utils;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;

namespace ITStepFinalProject
{
    public class Program
    {
        public static HashAlgorithm hashing;

        private static JWTHandler JWT;

        public static void Main(string[] args)
        {
            hashing = SHA256.Create();
            

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthorization();

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
            JWT = new JWTHandler(secretKey);

            builder.Services.AddScoped<JWTHandler>(_ => JWT);

            builder.Services.AddRateLimiter(_ => _
                .AddFixedWindowLimiter(policyName: "fixed", options => {
                    options.PermitLimit = 4;
                    options.Window = TimeSpan.FromSeconds(12);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 2;
                })
            );



            var app = builder.Build();


            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseRateLimiter();

            app.Use(async (HttpContext context, RequestDelegate next) =>
            {
                string path = context.Request.Path;
                IRequestCookieCollection cookies = context.Request.Cookies;
              
                string? _jwt = Utils.Utils.Get_JWT_FromCookie(cookies);
                bool isDateExp = await Utils.Utils.IsDateExpired(JWT, _jwt, 
                    "UserId_ExpirationDate");

                if (isDateExp) {
                    context.Response.Redirect(uri + "/login");
                    return;
                }

                
                if (context.Request.Method.ToUpper().Equals("GET") && 
                
                (File.Exists($"wwwroot{path}") || 
                File.Exists($"wwwroot{path}/Index.html"))) {
                    context.Response.Clear();
                    if (path.Contains('.')) {
                        await context.Response.WriteAsync(
                            File.ReadAllText($"wwwroot{path}"));

                    } else {
                        await context.Response.WriteAsync(
                            File.ReadAllText($"wwwroot{path}/Index.html"));
                    }
                    return;
                }

                await next.Invoke(context);
            });


            new UserController(app);
            new DishController(app);


            app.Run();
        }
    }
}
