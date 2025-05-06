using RestaurantSystem.Utils;

namespace RestaurantSystem.Controllers {
    public class ErrorController {


        public ErrorController(WebApplication app) {

            app.MapGet("/_restaurant_error", async (ControllerUtils controllerUtils) => {
                try {

                    string data = await controllerUtils.GetHTMLFromWWWROOT("/_restaurant_error");
                    return Results.Content(data, "text/html");

                } catch (Exception) {
                    return Results.BadRequest();
                }
            });

        }
    }
}
