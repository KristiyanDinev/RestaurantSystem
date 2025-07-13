using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace RestaurantSystem.Extentions
{
    public static class RateLimitExtention
    {

        public static void UseRateLimits(this IHostBuilder host)
        {
            host.ConfigureServices(
                s => s.AddRateLimiter(_ => _
                    .AddFixedWindowLimiter(policyName: "fixed", options =>
                    {
                        options.PermitLimit = 1;
                        options.Window = TimeSpan.FromSeconds(1);
                        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        options.QueueLimit = 2;
                    })
                )
            );
        }
    }
}
