using ITStepFinalProject.Database;
using ITStepFinalProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace ITStepFinalProject.Controllers {
    public class DishController {

        public DishController(WebApplication app) {


            app.MapGet("/dishes", async (HttpContext context,
                DatabaseManager db) => {

                    ISession session = context.Session;
                    if (session.GetInt32("UserId") == null) {
                        return Results.Redirect("/login");
                    }

                    try {
                        string data = await Utils.Utils.GetFileContent("/dishes");
                        return Results.Content(data, "text/html");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }
                    

                }).RequireRateLimiting("fixed");




            app.MapGet("/dishes/{type}", async (HttpContext context,
                DatabaseManager db, string type) => {

                    if (Utils.Utils.IsLoggedIn(context.Session) == null) {
                        return Results.Redirect("/login");
                    }

                    if (type.Contains('.')) {
                        string a = await Utils.Utils.GetFileContent("/dishes/" + type);
                        return Results.Content(a);
                    }

                    try {

                        string data = await Utils.Utils.GetFileContent("/dishes/" + type);
                        return Results.Content(data, "text/html");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }

                });



            app.MapGet("/dishes/id/{dishId}", async (HttpContext context,
                DatabaseManager db, string dishId) => {

                    if (Utils.Utils.IsLoggedIn(context.Session) == null) {
                        return Results.Redirect("/login");
                    }

                    if (!int.TryParse(dishId, out int Id)) {
                        return Results.Redirect("/dishes");
                    }

                    try {
                        string FileData = await Utils.Utils.GetFileContent("/single_dish");

                        DishModel dish = await db.GetDishById(Id);

                        Utils.Utils._handleEntryInFile(ref FileData, dish);

                        return Results.Content(FileData, "text/html");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }

                }).RequireRateLimiting("fixed");




            app.MapPost("/dishes", async (HttpContext context,
                DatabaseManager db, [FromForm] string type) => {

                    Dictionary<string, List<DishModel>> data = 
                    new Dictionary<string, List<DishModel>>();

                    try {

                        if (type.Equals("") || Utils.Utils.IsLoggedIn(context.Session) == null) {
                            throw new Exception();
                        }

                        List<DishModel> dishes = await db.GetDishes(type);
                        data.Add("dishes", dishes);
                        return data;

                    } catch (Exception) {
                        return data;
                    }
                }).RequireRateLimiting("fixed")
                .DisableAntiforgery();
        }
    }
}
