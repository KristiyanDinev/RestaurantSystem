using System;

namespace RestaurantSystem.Extentions
{
    public static class WebSocketExtention
    {

        public static void UseWebSockets(this IApplicationBuilder app, string uri)
        {
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(1),
                AllowedOrigins = { uri }
            });
        }
    }
}
