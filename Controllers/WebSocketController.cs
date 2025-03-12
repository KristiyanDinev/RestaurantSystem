using ITStepFinalProject.Controllers.WebSocketHandlers;
using ITStepFinalProject.Database.Handlers;

namespace ITStepFinalProject.Controllers
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
