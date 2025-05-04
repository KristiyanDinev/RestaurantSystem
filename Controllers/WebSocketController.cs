using RestaurantSystem.Database.Handlers;
using RestaurantSystem.Controllers;

namespace RestaurantSystem.Controllers
{
    public class WebSocketController
    {
        
        public WebSocketController(WebApplication app)
        {
            app.MapGet("/ws/orders", async (HttpContext context, WebSocketHandler webSocketHandler,
                OrderDatabaseHandler orderDatabaseHandler, DishDatabaseHandler dishDatabaseHandler) =>
            {
                await webSocketHandler.HandleWholeRequest(context, orderDatabaseHandler, dishDatabaseHandler);
            }).RequireRateLimiting("fixed");

            app.MapGet("/ws/reservations", async (HttpContext context, WebSocketHandler webSocketHandler, 
                OrderDatabaseHandler orderDatabaseHandler, DishDatabaseHandler dishDatabaseHandler) =>
            {
                await webSocketHandler.HandleWholeRequest(context, orderDatabaseHandler, dishDatabaseHandler);
            }).RequireRateLimiting("fixed");

            app.MapGet("/ws/cook", async (HttpContext context, WebSocketHandler webSocketHandler,
                OrderDatabaseHandler orderDatabaseHandler, DishDatabaseHandler dishDatabaseHandler) =>
            {
                await webSocketHandler.HandleWholeRequest(context, orderDatabaseHandler, dishDatabaseHandler);
            }).RequireRateLimiting("fixed");
        }

        
    }
}
