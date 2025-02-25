using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Xml.Linq;

namespace ITStepFinalProject.Controllers {
    public class DishController {

        public DishController(WebApplication app) {

           
            app.MapGet("/dishes", async (HttpContext context,
                ControllerUtils controllerUtils) =>
            {

                return await controllerUtils.HandleDefaultPage_WithUserModel("/dishes",
                          context);
            });


            
            app.MapGet("/dish/", async (HttpContext context,
                DishDatabaseHandler db, ControllerUtils controllerUtils, 
                [FromQuery(Name = "type")] string type) => {

                    try
                    {
                        UserModel? user = await controllerUtils.GetUserModelFromAuth(context);
                        if (user == null)
                        {
                            return Results.Redirect("/login");
                        }

                        List<DishModel> dishes = await db.GetDishes(type);
                        string FileData = await controllerUtils.GetFileContent("/dishes/" + type);

                        FileData = WebHelper.HandleCommonPlaceholders(FileData, "User", [user]);
                        FileData = WebHelper.HandleCommonPlaceholders(FileData, "Dish", dishes.Cast<object>()
                            .ToList());

                        return Results.Content(FileData, "text/html");
                    } catch (Exception)
                    {
                        return Results.Redirect("/error");
                    }

            }).RequireRateLimiting("fixed");



            app.MapGet("/dish/id", async (HttpContext context,
                DishDatabaseHandler dishDb, ControllerUtils controllerUtils, 
                [FromQuery(Name = "dishId")] string dishId) => {

                    if (!int.TryParse(dishId, out int Id)) {
                        return Results.Redirect("/dishes");
                    }

                    try {
                        
                        UserModel? user = await controllerUtils.GetUserModelFromAuth(context);
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
                        string FileData = await controllerUtils.GetFileContent("/single_dish");

                        

                        FileData = WebHelper.HandleCommonPlaceholders(FileData, "User", [user]);
                        FileData = WebHelper.HandleCommonPlaceholders(FileData, "Dish", [dish]);

                        return Results.Content(FileData, "text/html");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }

            }).RequireRateLimiting("fixed");
        }
    }
}
