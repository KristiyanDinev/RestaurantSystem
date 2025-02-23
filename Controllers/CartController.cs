using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using System.Text.Json;

namespace ITStepFinalProject.Controllers
{
    public class CartController
    {

        public CartController(WebApplication app)
        {
            // get current dishes about to order
            app.MapGet("/cart", async (HttpContext context, DishDatabaseHandler db) =>
            {
                try
                {
                    ISession session = context.Session;
                    int? id = ControllerUtils.IsLoggedIn(context.Session);
                    if (id == null)
                    {
                        return Results.Redirect("/login");
                    }

                    List<int> dishesIds = OrderControllerUtils.GetDishesFromOrder(session);
                    if (dishesIds.Count == 0)
                    {
                        return Results.Redirect("/dishes");
                    }

                    string FileData = await ControllerUtils.GetFileContent("/cart");

                    UserModel user = ControllerUtils.GetModelFromSession(session, "User").Deserialize<UserModel>(); ;

                    List<DishModel> dishes = await db.GetDishesByIds(dishesIds.ToHashSet().ToList());

                    List<DisplayDishModel> displayDishModels = new List<DisplayDishModel>();

                    foreach (DishModel dishModel in dishes)
                    {
                        displayDishModels.Add(new DisplayDishModel(dishModel));
                    }

                    foreach (int dishId in dishesIds)
                    {
                        foreach (DisplayDishModel dishModel in displayDishModels)
                        {
                            if (dishModel.Id == dishId)
                            {
                                dishModel.Amount += 1;
                            }
                        }
                    }


                    FileData = WebHelper.HandleCommonPlaceholders(FileData, "User", [user]);
                    FileData = WebHelper.HandleCommonPlaceholders(FileData, "Dish", displayDishModels.Cast<object>()
                        .ToList());

                    FileData = WebHelper.HandleCommonPlaceholders(FileData, "Restorant", 
                        ControllerUtils.GetRestorantsForUser(user).Cast<object>().ToList());

                    return Results.Content(FileData, "text/html");

                }
                catch (Exception)
                {
                    return Results.Redirect("/error");
                }
            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();
        }
    }
}
