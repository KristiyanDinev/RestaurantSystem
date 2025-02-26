﻿using ITStepFinalProject.Database.Handlers;
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
            app.MapGet("/cart", async (HttpContext context, DishDatabaseHandler db,
                ControllerUtils controllerUtils, WebUtils webUtils, UserUtils userUtils) =>
            {
                try {

                    UserModel? user = await userUtils.GetUserModelFromAuth(context);
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

                    FileData = webUtils.HandleCommonPlaceholders(FileData, 
                        controllerUtils.UserModelName, [user]);

                    FileData = webUtils.HandleCommonPlaceholders(FileData, 
                        controllerUtils.DishModelName, displayDishModels.Cast<object>()
                        .ToList());

                    FileData = webUtils.HandleCommonPlaceholders(FileData, 
                        controllerUtils.RestorantModelName,
                        userUtils.GetRestorantsForUser(user).Cast<object>().ToList());

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
