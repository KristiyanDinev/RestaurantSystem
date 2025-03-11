using ITStepFinalProject.Controllers.WebSocketHandlers;
using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils.Controller;
using System.Net.WebSockets;

namespace ITStepFinalProject.Controllers
{
    public class WebSocketController
    {
        
        public WebSocketController(WebApplication app)
        {
            app.MapGet("/ws/orders", async (HttpContext context, WebSocketHandler webSocketHandler,
                OrderDatabaseHandler orderDatabaseHandler) =>
            {
                await webSocketHandler.HandleWholeRequest(context, orderDatabaseHandler);
            }).RequireRateLimiting("fixed");

            app.MapGet("/ws/reservations", async (HttpContext context, WebSocketHandler webSocketHandler, 
                OrderDatabaseHandler orderDatabaseHandler) =>
            {
                await webSocketHandler.HandleWholeRequest(context, orderDatabaseHandler);
            }).RequireRateLimiting("fixed");

            app.MapGet("/ws/cook", async (HttpContext context, WebSocketHandler webSocketHandler,
                OrderDatabaseHandler orderDatabaseHandler) =>
            {
                await webSocketHandler.HandleWholeRequest(context, orderDatabaseHandler);
            }).RequireRateLimiting("fixed");
        }

        
    }
}
