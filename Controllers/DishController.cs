using ITStepFinalProject.Database;
using ITStepFinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ITStepFinalProject.Controllers {
    public class DishController {

        public DishController(WebApplication app) {


            app.MapGet("/dishes", async (HttpContext context,
                DatabaseManager db) => {

                    int? id = Utils.Utils.IsLoggedIn(context.Session);
                    if (id == null) {
                        return Results.Redirect("/login");
                    }

                    try {
                        string data = await Utils.Utils.GetFileContent("/dishes");

                        UserModel user = await db.GetUser((int)id);
                        Utils.Utils.ApplyUserBarElement(ref data, user);
                        Utils.Utils._handleEntryInFile(ref data, user, "User");

                        return Results.Content(data, "text/html");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }
                    

                }).RequireRateLimiting("fixed");




            app.MapGet("/dishes/{type}", async (HttpContext context,
                DatabaseManager db, string type) => {

                    int? id = Utils.Utils.IsLoggedIn(context.Session);
                    if (id == null) {
                        return Results.Redirect("/login");
                    }

                    if (type.Contains('.')) {
                        // just static file
                        string a = await Utils.Utils.GetFileContent("/dishes/" + type);
                        return Results.Content(a);
                    }

                    try {

                        string data = await Utils.Utils.GetFileContent("/dishes/" + type);

                        UserModel user = await db.GetUser((int)id);
                        Utils.Utils._handleEntryInFile(ref data, user, "User");
                        Utils.Utils.ApplyUserBarElement(ref data, user);

                        return Results.Content(data, "text/html");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }

                });



            app.MapGet("/dishes/id/{dishId}", async (HttpContext context,
                DatabaseManager db, string dishId) => {

                    int? id = Utils.Utils.IsLoggedIn(context.Session);
                    if (id == null) {
                        return Results.Redirect("/login");
                    }

                    if (!int.TryParse(dishId, out int Id)) {
                        return Results.Redirect("/dishes");
                    }

                    try {
                        string FileData = await Utils.Utils.GetFileContent("/single_dish");

                        DishModel dish = await db.GetDishById(Id);

                        Utils.Utils._handleEntryInFile(ref FileData, dish, "Dish");

                        UserModel user = await db.GetUser((int)id);
                        Utils.Utils._handleEntryInFile(ref FileData, user, "User");
                        Utils.Utils.ApplyUserBarElement(ref FileData, user);


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
