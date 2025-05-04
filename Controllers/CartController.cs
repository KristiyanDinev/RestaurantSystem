using RestaurantSystem.Database.Handlers;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.WebModels;
using RestaurantSystem.Utils.Controller;
using RestaurantSystem.Utils.Web;

namespace RestaurantSystem.Controllers
{
    public class CartController
    {

        public CartController(WebApplication app)
        {
            
            // get current dishes about to order
            app.MapGet("/cart", async (HttpContext context, DishDatabaseHandler db,
                OrderDatabaseHandler orderDB,
                ControllerUtils controllerUtils, WebUtils webUtils, UserUtils userUtils) =>
            {
                try {

                    if (!int.TryParse(context.Request.Cookies[controllerUtils.RestoratIdHeaderName], 
                        out int restorantId))
                    {
                        return Results.Redirect("/dishes");
                    }

                    UserModel? user = await userUtils.GetUserByJWT(context);
                    if (user == null)
                    {
                        return Results.Redirect("/login");
                    }

                    string FileData = await controllerUtils.GetHTMLFromWWWROOT("/cart");

                    List<int> dishesIds = controllerUtils.GetCartItems(context);

                    List<DishModel> dishes = new List<DishModel>();
                    if (dishesIds.Count > 0)
                    {
                        dishes = await db.GetDishesByIds(dishesIds.ToHashSet().ToList());
                    }

                    TimeTableJoinRestorantModel restorantAddress =
                        await orderDB.GetRestorantAddressById(restorantId);

                    List<DishModel> temp = new List<DishModel>();
                    foreach (int id in dishesIds)
                    {
                        foreach (DishModel dish in dishes)
                        {
                            if (dish.Id == id)
                            {
                                temp.Add(dish);
                                break;
                            }
                        }
                    }

                    List<DisplayDishModel> displayDishModels = controllerUtils.ConvertToDisplayDish(temp);

                    FileData = webUtils.HandleCommonPlaceholders(FileData, 
                        controllerUtils.UserModelName, [user]);

                    FileData = webUtils.HandleCommonPlaceholders(FileData, 
                        controllerUtils.DishModelName, displayDishModels.Cast<object>()
                        .ToList());

                    FileData = webUtils.HandleCommonPlaceholders(FileData,
                        controllerUtils.RestorantModelName, [restorantAddress]);

                    return Results.Content(FileData, "text/html");

                }
                catch (Exception)
                {
                    return Results.Redirect("/dishes");
                }
            }).RequireRateLimiting("fixed")
            .DisableAntiforgery();
        }
    }
}
