using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ITStepFinalProject.Controllers {
    public class OrderController {


        public OrderController(WebApplication app) {

            // get front-end display of your orders
            app.MapGet("/orders", async (HttpContext context, 
                OrderDatabaseHandler db, ControllerUtils controllerUtils) => {

                try {
                        UserModel? user = await controllerUtils.GetUserModelFromAuth(context);
                        if (user == null)
                        {
                            return Results.Redirect("/login");
                        }

                        List<OrderModel> orders = await db.GetOrdersByUser(user.Id);

                        string FileData = await controllerUtils.GetFileContent("/orders");

                        FileData = WebHelper.HandleCommonPlaceholders(FileData, "User", [user]);
                        FileData = WebHelper.HandleCommonPlaceholders(FileData, "Order", orders.Cast<object>().ToList());

                    return Results.Content(FileData, "text/html");

                } catch (Exception) {
                    return Results.BadRequest();
                }
            }).RequireRateLimiting("fixed");


            app.MapPost("/order/dishes", async (HttpContext context,
                OrderDatabaseHandler db,
                [FromForm] int orderId) => {

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


            // add dish  (depricated/maybe replace it with cookies on the client side)
            app.MapPost("/order/add", async (HttpContext context, 
                 [FromForm] int dishId) => {

                    try {
                        


                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }
                     
            }).RequireRateLimiting("fixed")
              .DisableAntiforgery();


            // remove dish (depricated/maybe replace it with cookies on the client side)
            app.MapPost("/order/remove", async (HttpContext context,
               [FromForm] int dishId) => {

                    try {

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }

                }).RequireRateLimiting("fixed")
              .DisableAntiforgery();

            /*
            // start order
            app.MapPost("/order", async (HttpContext context,
                CuponDatabaseHandler cuponDb, OrderDatabaseHandler orderDb,
                ControllerUtils controllerUtils,
                [FromForm] string notes,
                [FromForm] string cuponCode, [FromForm] string restorantAddress) => {

                    try {

                        UserModel user = await controllerUtils.GetUserModelFromAuth(context);

                        List<float> currentPrices = OrderControllerUtils.GetPricesFromOrder(session);
                        decimal TotalPrice = OrderControllerUtils.CalculateTotalPrice(currentPrices, 0);

                        CuponModel? cupon = null;
                        if (cuponCode.Length > 0) {
                            cupon = await cuponDb.GetCuponByCode(cuponCode);

                            if (cupon != null)
                            {
                                if (cupon.ExpirationDate.ToLocalTime() <= DateTime.Now) {
                                    return Results.BadRequest();
                                }

                                TotalPrice = OrderControllerUtils.CalculateTotalPrice(currentPrices,
                                    cupon.DiscountPercent);
                            }
                        }

                        InsertOrderModel order = new InsertOrderModel();
                        order.RestorantAddress = restorantAddress;
                        order.UserId = user.Id;
                        order.TotalPrice = TotalPrice;
                        order.Notes = notes;

                        orderDb.AddOrder(ControllerUtils.GetModelFromSession(session, "User").Deserialize<UserModel>(),
                            
                            OrderControllerUtils.GetDishesFromOrder(session), order);

                        if (cupon != null) {
                            cuponDb.DeleteCupon(cupon.CuponCode);
                        }

                        OrderControllerUtils.DeleteOrderFromSession(ref session);

                        await session.CommitAsync();

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }

                }).RequireRateLimiting("fixed")
              .DisableAntiforgery();
            */

            // stop order
            app.MapPost("/order/stop", async (HttpContext context,
                OrderDatabaseHandler db, [FromForm] int orderId) => {

                    try {
                        string? status = 
                            await db.GetOrder_CurrentStatus_ById(orderId);

                        if (status == null || !(status.Equals("pending") || 
                            status.Equals("db"))) {
                            return Results.BadRequest();
                        }

                        db.DeleteOrder(orderId);

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }

                }).RequireRateLimiting("fixed")
              .DisableAntiforgery();
        }
    }
}
