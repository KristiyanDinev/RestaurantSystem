using ITStepFinalProject.Controllers;
using ITStepFinalProject.Database;
using Microsoft.AspNetCore.RateLimiting;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;

namespace ITStepFinalProject
{
    public class Program
    {
        public static HashAlgorithm? hashing;
        public static void Main(string[] args)
        {
            hashing = SHA256.Create();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();

            // postgresql 17 5432
            // postgresql 16 5433
            DatabaseConnection._connectionString = 
                builder.Configuration.GetValue<string>("ConnectionString") ?? "";

            DatabaseManager.Setup();

            builder.Services.AddScoped<DatabaseManager>();
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
                //context.Request.HttpContext.Response.Redirect("https://stackoverflow.com/");
                //Console.WriteLine(context.Request.Path); -> /dishes
                string path = context.Request.Path;


                if (context.Request.Method.ToUpper().Equals("GET") && 
                
                (File.Exists($"wwwroot{path}") || 
                File.Exists($"wwwroot{path}/Index.html"))) {

                    if (path.Contains('.')) {
                        await context.Request.HttpContext
                        .Response.WriteAsync(
                            File.ReadAllText($"wwwroot{path}"));

                    } else {
                        await context.Request.HttpContext
                        .Response.WriteAsync(
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
