using RestaurantSystem.Database.Handlers;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Utils.Controller;
using RestaurantSystem.Utils.Web;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantSystem.Controllers {
    public class DishController {

        public DishController(WebApplication app) {

           
            app.MapGet("/dishes", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils, 
                OrderDatabaseHandler orderDB) =>
            {

                try
                {
                    UserModel? user = await userUtils.GetUserByJWT(context);
                    if (user == null)
                    {
                        return Results.Redirect("/login");
                    }

                    List<TimeTableJoinRestorantModel> timeTableWithRestorantAddresses =
                        await orderDB.GetRestorantsAddressesForUser(user);

                    string FileData = await controllerUtils.GetHTMLFromWWWROOT("/dishes");

                    FileData = webUtils.HandleCommonPlaceholders(FileData, 
                        controllerUtils.UserModelName, [user]);

                    FileData = webUtils.HandleCommonPlaceholders(FileData,
                        controllerUtils.RestorantModelName, timeTableWithRestorantAddresses
                        .Cast<object>().ToList());

                    return Results.Content(FileData, "text/html");

                }
                catch (Exception)
                {
                    return Results.Redirect("/error");
                }
            });


            
            app.MapGet("/dish", async (HttpContext context,
                DishDatabaseHandler db, ControllerUtils controllerUtils, 
                UserUtils userUtils, WebUtils webUtils,
                [FromQuery(Name = "type")] string type) => {

                    try
                    {
                        if (!int.TryParse(context.Request.Cookies[controllerUtils.RestoratIdHeaderName],
                            out int restorant_id_num))
                        {
                            return Results.Redirect("/dishes");
                        }

                        UserModel? user = await userUtils.GetUserByJWT(context);
                        if (user == null)
                        {
                            return Results.Redirect("/login");
                        }

                        List<DishModel> dishes = await db.GetDishes(type, restorant_id_num);
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

//  TODO

            app.MapGet("/single_dish", async (HttpContext context,
                DishDatabaseHandler dishDb, ControllerUtils controllerUtils, 
                OrderDatabaseHandler orderDatabaseHandler,
                UserUtils userUtils, WebUtils webUtils,
                [FromQuery(Name = "dishId")] string dishId) => {
                    
                    try {

                        if (!int.TryParse(dishId, out int Id))
                        {
                            return Results.Redirect("/dishes");
                        }

                        UserModel? user = await userUtils.GetUserByJWT(context);
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

                        List<TimeTableJoinRestorantModel> dishAvailableInRestorants =
                                await orderDatabaseHandler.GetRestorantAddressesWhere_DishId_IsAvailable(Id);

                        FileData = webUtils.HandleCommonPlaceholders(FileData, 
                            controllerUtils.UserModelName, [user]);

                        FileData = webUtils.HandleCommonPlaceholders(FileData, 
                            controllerUtils.DishModelName, [dish]);

                        FileData = webUtils.HandleCommonPlaceholders(FileData,
                            controllerUtils.RestorantModelName, dishAvailableInRestorants
                            .Cast<object>().ToList());

                        return Results.Content(FileData, "text/html");

                    } catch (Exception) {
                        return Results.Redirect("/error");
                    }

            }).RequireRateLimiting("fixed");
        }
    }
}
