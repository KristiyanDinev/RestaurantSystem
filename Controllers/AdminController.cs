using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Utils.Controller;
using ITStepFinalProject.Utils.Web;

namespace ITStepFinalProject.Controllers
{
    public class AdminController
    {
        public AdminController(WebApplication app)
        {

            // Note: Here should be only endpoints that are for staff.
            // These endpoints are already Authorized by the Authentication middleware.
            
            app.MapGet("/admin", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils, 
                ServiceDatabaseHandler serviceDatabaseHandler) => {

                    try
                    {
                        UserModel? user = await userUtils.GetUserModelFromAuth(context);
                        if (user == null)
                        {
                            return Results.Redirect("/login");
                        }

                        string FileData = await controllerUtils.GetHTMLFromWWWROOT("/admin");

                        RestorantModel restorantModel = await serviceDatabaseHandler.GetStaffRestorant(user);

                        FileData = webUtils.HandleCommonPlaceholders(FileData, controllerUtils.UserModelName, [user]);

                        FileData = webUtils.HandleCommonPlaceholders(FileData, 
                            controllerUtils.RestorantModelName, [restorantModel]);

                        return Results.Content(FileData, "text/html");

                    }
                    catch (Exception)
                    {
                        return Results.Redirect("/error");
                    }
                });


            app.MapGet("/admin/dishes", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils) => {

                    return await controllerUtils.HandleDefaultPage_WithUserModel("/admin/cook",
                          context, userUtils, webUtils);
                });

            app.MapGet("/admin/orders", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils) => {

                    return await controllerUtils.HandleDefaultPage_WithUserModel("/admin/waitress",
                          context, userUtils, webUtils);
                });

            app.MapGet("/admin/delivery", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils) => {

                    return await controllerUtils.HandleDefaultPage_WithUserModel("/admin/delivery",
                          context, userUtils, webUtils);
                });

            app.MapGet("/admin/owner", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils) => {

                    return await controllerUtils.HandleDefaultPage_WithUserModel("/admin/owner",
                          context, userUtils, webUtils);
                });
        }
    }
}
