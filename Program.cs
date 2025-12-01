using RestaurantSystem.Extentions;
using RestaurantSystem.Middlewares;
using RestaurantSystem.Models.WebSockets;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;
using Serilog;
using System.Net.WebSockets;

namespace RestaurantSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            string? Encryption_Key = builder.Configuration.GetValue<string>("Encryption_Key");
            string? Hash_Salt = builder.Configuration.GetValue<string>("Hash_Salt");
            string? JWT_Key = builder.Configuration.GetValue<string>("JWT_Key");

            if (Encryption_Key == null || Hash_Salt == null || JWT_Key == null)
            {
                throw new Exception("Encryption_Key, Hash_Salt, or JWT_Key is not set in configuration.");
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog((context, loggerConfig) =>
            {
                loggerConfig.ReadFrom.Configuration(context.Configuration);
            });

            Log.Information("Current Working Directory: " + Directory.GetCurrentDirectory());
            Log.Information("Configuring");

            builder.Services.Configure<RabbitMQOptionsModel>(
                builder.Configuration.GetSection("RabbitMQ"));

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
            builder.Services.AddScoped<CouponService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<RoleService>();
            builder.Services.AddScoped<ReservationService>();
            builder.Services.AddScoped<LocationService>();
            builder.Services.AddScoped<AddressService>();
            builder.Services.AddScoped<DeliveryService>();

            builder.Services.AddScoped<EncryptionUtility>(_ =>
                new EncryptionUtility(Encryption_Key, Hash_Salt));

            builder.Services.AddScoped<JWTUtility>(_ =>
                new JWTUtility(JWT_Key));

            builder.Services.AddScoped<UserUtility>();
            builder.Services.AddScoped<WebSocketDatabaseService>();

            builder.Services.AddScoped<EmailService>();
            builder.Services.AddSingleton<EmailSendService>(_ => new EmailSendService(builder.Configuration));

            builder.Services.AddSingleton<WebSocketUtility>();
            builder.Services.AddScoped<WebSocketService>();
            builder.Host.UseRateLimits();

            WebApplication app = builder.Build();

            string ipPort = uri.Split("/").Last();
            string[] parts = ipPort.Split(":");
            string ip = parts.First();
            string port = parts.Last();
            // Security headers middleware
            app.Use(async (context, next) =>
            {
                // Content Security Policy
                context.Response.Headers.Add(
                    "Content-Security-Policy",
                    "default-src 'self';" +
                    "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://stackpath.bootstrapcdn.com;" +
                    "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://stackpath.bootstrapcdn.com;" +
                    "font-src 'self' https://cdn.jsdelivr.net;" +
                    "img-src 'self' data: https:;" +
                    "connect-src 'self' " +
                        "wss://" + ipPort + " " +
                        "ws://" + ipPort + " " +
                        "ws://localhost:" + port + " " +
                        "wss://localhost:" + port + ";"
                );

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
            app.MapControllers();
            app.UseStartupSQL(app);

            app.Lifetime.ApplicationStopping.Register(() =>
            {
                Task.Run(async () =>
                {
                    using IServiceScope scope = app.Services.CreateScope();
                    WebSocketService webSocketService = scope
                            .ServiceProvider.GetRequiredService<WebSocketService>();

                    WebSocketUtility webSocketUtility = scope
                            .ServiceProvider.GetRequiredService<WebSocketUtility>();
                    foreach (WebSocket webSocket in webSocketUtility.GetOrderWebSocketModels().Select(m => m.Socket))
                    {
                        try
                        {
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        }
                        catch { }
                    }
                    string _serverId = webSocketUtility.GetServerId();
                    if (_serverId != null)
                    {
                        await scope.ServiceProvider.GetRequiredService<WebSocketDatabaseService>()
                            .DeleteAllOrderServerMappingByServerIdAsync(_serverId);
                    }

                }).GetAwaiter().GetResult();
            });


            app.Run();
        }
    }
}
