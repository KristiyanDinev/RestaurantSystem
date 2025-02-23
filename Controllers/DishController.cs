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
                UserDatabaseHandler db) =>
            {

                return await ControllerUtils.HandleDefaultPage_WithUserModel("/dishes",
                          context, db);
            });



            app.MapGet("/dish/", async (HttpContext context,
                DishDatabaseHandler db, [FromQuery(Name = "type")] string type) => {

                    ISession session = context.Session;
                    int? id = ControllerUtils.IsLoggedIn(context.Session);
                    if (id == null)
                    {
                        return Results.Redirect("/login");
                    }

                    try
                    {
                        List<DishModel> dishes = await db.GetDishes(type);
                        string FileData = await ControllerUtils.GetFileContent("/dishes/" + type);

                        UserModel user = ControllerUtils.GetModelFromSession(session, "User").Deserialize<UserModel>(); ;

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
                DishDatabaseHandler dishDb, [FromQuery(Name = "dishId")] string dishId) => {

                    ISession session = context.Session;
                    int? id = ControllerUtils.IsLoggedIn(session);
                    if (id == null) {
                        return Results.Redirect("/login");
                    }

                    if (!int.TryParse(dishId, out int Id)) {
                        return Results.Redirect("/dishes");
                    }

                    try {

                        List<DishModel> dishes = await dishDb.GetDishesByIds([Id]);
                        if (dishes.Count == 0)
                        {
                            throw new Exception();
                        }

                        DishModel dish = dishes[0];
                        string FileData = await ControllerUtils.GetFileContent("/single_dish");

                        UserModel user = ControllerUtils.GetModelFromSession(session, "User").Deserialize<UserModel>(); ;

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
