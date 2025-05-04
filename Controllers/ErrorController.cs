using RestaurantSystem.Utils.Controller;

namespace RestaurantSystem.Controllers {
    public class ErrorController {


        public ErrorController(WebApplication app) {

            app.MapGet("/error", async (ControllerUtils controllerUtils) => {
                try {

                    string data = await controllerUtils.GetHTMLFromWWWROOT("/error");
                    return Results.Content(data, "text/html");

                } catch (Exception) {
                    return Results.BadRequest();
                }
            });

        }
    }
}
