using ITStepFinalProject.Database;
using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;

namespace ITStepFinalProject.Controllers
{
    public class CartController
    {

        public CartController(WebApplication app)
        {
            app.MapGet("/cart", async (HttpContext context, 
                UserDatabaseHandler db) =>
            {
                return await ControllerUtils.HandleDefaultPage("/cart", context, db, true);
            });
        }
    }
}
