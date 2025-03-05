using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels;
using ITStepFinalProject.Models.WebModels;
using ITStepFinalProject.Utils.Controller;
using ITStepFinalProject.Utils.Web;
using Microsoft.AspNetCore.Mvc;

namespace ITStepFinalProject.Controllers {
    public class OrderController {


        public OrderController(WebApplication app) {

            // get front-end display of your orders
            app.MapGet("/orders", async (HttpContext context, 
                OrderDatabaseHandler db, ControllerUtils controllerUtils, 
                UserUtils userUtils, WebUtils webUtils) => {

                try {
                        UserModel? user = await userUtils.GetUserModelFromAuth(context);
                        if (user == null)
                        {
                            return Results.Redirect("/login");
                        }

                        List<DisplayOrderModel> orders = await db.GetOrdersByUser(user.Id);

                        string FileData = await controllerUtils.GetHTMLFromWWWROOT("/orders");

                        FileData = webUtils.HandleCommonPlaceholders(FileData, 
                            controllerUtils.UserModelName, [user]);

                        FileData = webUtils.HandleCommonPlaceholders(FileData, 
                            controllerUtils.OrderModelName, orders.Cast<object>().ToList());

                    return Results.Content(FileData, "text/html");

                } catch (Exception) {
                    return Results.BadRequest();
                }
            }).RequireRateLimiting("fixed");


            app.MapPost("/order/dishes", async (HttpContext context,
                OrderDatabaseHandler db,
                [FromForm] string orderIdStr) => {

                    if (!int.TryParse(orderIdStr, out int orderId))
                    {
                        return Results.BadRequest();
                    }

                    try
                    {

                        List<DishModel> dishes = await db.GetAllDishesFromOrder(orderId);

                        return Results.Ok(new Dictionary<string, List<DishModel>>
                        {
                            {"dishes", dishes }
                        });

                    } catch (Exception)
                    {
                        return Results.BadRequest();
                    }

            }).RequireRateLimiting("fixed")
              .DisableAntiforgery();


           

            
            // start order
            app.MapPost("/order", async (HttpContext context,
                CuponDatabaseHandler cuponDb, OrderDatabaseHandler orderDb,
                DishDatabaseHandler dishDb,
                ControllerUtils controllerUtils, UserUtils userUtils,
                [FromForm] string notes,
                [FromForm] string cuponCode, [FromForm] string restorantIdStr) => {

                    try {

                        if (!int.TryParse(restorantIdStr, out int restorantId))
                        {
                            return Results.BadRequest();
                        }

                        List<int>? dishesIds = controllerUtils.GetCartItems(context);
                        if (dishesIds == null && dishesIds.Count == 0)
                        {
                            return Results.BadRequest();
                        }

                        UserModel? user = await userUtils.GetUserModelFromAuth(context);
                        if (user == null)
                        {
                            return Results.Redirect("/login");
                        }

                        List<DishModel> dishes = await dishDb.GetDishesByIds(dishesIds.ToHashSet().ToList());

                        decimal TotalPrice = OrderUtils.CalculateTotalPrice(dishes, 0);

                        CuponModel? cupon = null;
                        if (cuponCode.Length > 0) {
                            cupon = await cuponDb.GetCuponByCode(cuponCode);

                            if (cupon != null)
                            {
                                if (cupon.ExpirationDate.ToLocalTime() <= DateTime.Now) {
                                    return Results.BadRequest();
                                }

                                TotalPrice = OrderUtils.CalculateTotalPrice(dishes,
                                    cupon.DiscountPercent);
                            }
                        }

                        InsertOrderModel order = new InsertOrderModel();
                        order.RestorantId = restorantId;
                        order.UserId = user.Id;
                        order.TotalPrice = TotalPrice;
                        order.Notes = notes;

                        orderDb.AddOrder(user.Id, dishesIds, order, controllerUtils);

                        if (cupon != null) {
                            cuponDb.DeleteCupon(cupon.CuponCode);
                        }

                        context.Response.Cookies.Delete(controllerUtils.CartHeaderName);

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }

                }).RequireRateLimiting("fixed")
              .DisableAntiforgery();
            

            // stop order
            app.MapPost("/order/stop", async (HttpContext context,
                OrderDatabaseHandler db, WebSocketUtils webSocketUtils,
                ControllerUtils controllerUtils,
                [FromForm] string orderIdStr) => {

                    if (!int.TryParse(orderIdStr, out int orderId))
                    {
                        return Results.BadRequest();
                    }

                    try {
                        string? status = 
                            await db.GetOrder_CurrentStatus_ById(orderId);

                        if (status == null || !(status.Equals(controllerUtils.PendingStatus) || 
                            status.Equals(controllerUtils.DBStatus))) {
                            return Results.BadRequest();
                        }

                        db.DeleteOrder(orderId);

                        webSocketUtils.RemoveModelIdFromOrderSubscribtion(orderId);

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }

                }).RequireRateLimiting("fixed")
              .DisableAntiforgery();
        }
    }
}
