using ITStepFinalProject.Utils;

namespace ITStepFinalProject.Controllers {
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
