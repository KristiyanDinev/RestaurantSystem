using ITStepFinalProject.Utils.Controller;
using ITStepFinalProject.Utils.Web;

namespace ITStepFinalProject.Controllers
{
    public class AdminController
    {
        public AdminController(WebApplication app)
        {
            app.MapGet("/admin", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils) => {
                    return await controllerUtils.HandleDefaultPage_WithUserModel("/admin",
                          context, userUtils, webUtils);
                });
        }
    }
}
