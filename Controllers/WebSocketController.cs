using RestaurantSystem.Controllers;
using RestaurantSystem.Services;

namespace RestaurantSystem.Controllers
{
    public class WebSocketController
    {
        
        public WebSocketController(WebApplication app)
        {
            app.MapGet("/ws/orders", async (HttpContext context, WebSocketHandler webSocketHandler,
                OrderService orderDatabaseHandler, DishService dishDatabaseHandler) =>
            {
                await webSocketHandler.HandleWholeRequest(context, orderDatabaseHandler, dishDatabaseHandler);
            }).RequireRateLimiting("fixed");

            app.MapGet("/ws/Reservations", async (HttpContext context, WebSocketHandler webSocketHandler, 
                OrderService orderDatabaseHandler, DishService dishDatabaseHandler) =>
            {
                await webSocketHandler.HandleWholeRequest(context, orderDatabaseHandler, dishDatabaseHandler);
            }).RequireRateLimiting("fixed");

            app.MapGet("/ws/cook", async (HttpContext context, WebSocketHandler webSocketHandler,
                OrderService orderDatabaseHandler, DishService dishDatabaseHandler) =>
            {
                await webSocketHandler.HandleWholeRequest(context, orderDatabaseHandler, dishDatabaseHandler);
            }).RequireRateLimiting("fixed");
        }

        
    }
}
