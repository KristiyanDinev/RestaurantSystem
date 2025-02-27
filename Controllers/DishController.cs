using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Utils.Controller;
using ITStepFinalProject.Utils.Web;
using Microsoft.AspNetCore.Mvc;

namespace ITStepFinalProject.Controllers {
    public class DishController {

        public DishController(WebApplication app) {

           
            app.MapGet("/dishes", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils) =>
            {

                return await controllerUtils.HandleDefaultPage_WithUserModel("/dishes",
                          context, userUtils, webUtils);
            });


            
            app.MapGet("/dish/", async (HttpContext context,
                DishDatabaseHandler db, ControllerUtils controllerUtils, 
                UserUtils userUtils, WebUtils webUtils,
                [FromQuery(Name = "type")] string type) => {

                    try
                    {
                        UserModel? user = await userUtils.GetUserModelFromAuth(context);
                        if (user == null)
                        {
                            return Results.Redirect("/login");
                        }

                        List<DishModel> dishes = await db.GetDishes(type);
                        string FileData = await controllerUtils.GetHTMLFromWWWROOT("/dishes/" + type);

                        FileData = webUtils.HandleCommonPlaceholders(FileData, 
                            controllerUtils.UserModelName, [user]);

                        FileData = webUtils.HandleCommonPlaceholders(FileData, 
                            controllerUtils.DishModelName, dishes.Cast<object>()
                            .ToList());

                        return Results.Content(FileData, "text/html");
                    } catch (Exception)
                    {
                        return Results.Redirect("/error");
                    }

            }).RequireRateLimiting("fixed");



            app.MapGet("/dish/id", async (HttpContext context,
                DishDatabaseHandler dishDb, ControllerUtils controllerUtils, 
                UserUtils userUtils, WebUtils webUtils,
                [FromQuery(Name = "dishId")] string dishId) => {

                    if (!int.TryParse(dishId, out int Id)) {
                        return Results.Redirect("/dishes");
                    }

                    try {
                        
                        UserModel? user = await userUtils.GetUserModelFromAuth(context);
                        if (user == null)
                        {
                            return Results.Redirect("/login");
                        }

                        List<DishModel> dishes = await dishDb.GetDishesByIds([Id]);
                        if (dishes.Count == 0)
                        {
                            throw new Exception();
                        }

                        DishModel dish = dishes[0];
                        string FileData = await controllerUtils.GetHTMLFromWWWROOT("/single_dish");

                        

                        FileData = webUtils.HandleCommonPlaceholders(FileData, 
                            controllerUtils.UserModelName, [user]);

                        FileData = webUtils.HandleCommonPlaceholders(FileData, 
                            controllerUtils.DishModelName, [dish]);

                        return Results.Content(FileData, "text/html");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }

            }).RequireRateLimiting("fixed");
        }
    }
}
