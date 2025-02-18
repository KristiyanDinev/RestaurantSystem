namespace ITStepFinalProject.Controllers {
    public class ErrorController {


        public ErrorController(WebApplication app) {

            app.MapGet("/error", async () => {
                try {

                    string data = await Utils.ControllerUtils.GetFileContent("/error");
                    return Results.Content(data, "text/html");

                } catch (Exception) {
                    return Results.BadRequest();
                }
            });

        }
    }
}
