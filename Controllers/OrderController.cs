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
                OrderDatabaseHandler db) => {

                try {
                        ISession session = context.Session;

                        int? id = ControllerUtils.IsLoggedIn(session);
                        if (id == null) {
                            return Results.Unauthorized();
                        }

                        List<OrderModel> orders = await db.GetOrdersByUser((int)id);

                        string FileData = await ControllerUtils.GetFileContent("/orders");

                        UserModel user = ControllerUtils.GetModelFromSession(session, "User").Deserialize<UserModel>(); ;

                        FileData = WebHelper.HandleCommonPlaceholders(FileData, "User", [user]);
                        FileData = WebHelper.HandleCommonPlaceholders(FileData, "Order", orders.Cast<object>()
                            .ToList());


                        return Results.Content(FileData, "text/html");

                    } catch (Exception) {
                        return Results.BadRequest();
                    }
            }).RequireRateLimiting("fixed");


            // add dish
            app.MapPost("/order/add", async (HttpContext context, 
                 [FromForm] int dishId, 
                [FromForm] float dishPrice) => {

                    try {

                        ISession session = context.Session;
                        if (ControllerUtils.IsLoggedIn(session) == null) {
                            return Results.Unauthorized();
                        }

                        OrderControllerUtils.AddDishToOrder(ref session, dishId,
                            dishPrice);

                        await session.CommitAsync();

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }
                     
            }).RequireRateLimiting("fixed")
              .DisableAntiforgery();


            // remove dish
            app.MapPost("/order/remove", async (HttpContext context,
               [FromForm] int dishId) => {

                    try {

                        ISession session = context.Session;
                        if (ControllerUtils.IsLoggedIn(session) == null) {
                            return Results.Unauthorized();
                        }

                        OrderControllerUtils.RemoveOneDishByIDFromOrder(ref session, dishId);
                        await session.CommitAsync();

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }

                }).RequireRateLimiting("fixed")
              .DisableAntiforgery();

            // start order
            app.MapPost("/order", async (HttpContext context,
                CuponDatabaseHandler cuponDb, OrderDatabaseHandler orderDb, 
                [FromForm] string notes,
                [FromForm] string cuponCode, [FromForm] string restorantAddress) => {

                    try {

                        ISession session = context.Session;
                        int? userId = ControllerUtils.IsLoggedIn(session);
                        if (userId == null) {
                            return Results.Unauthorized();
                        }

                        
                        List<float> currentPrices = OrderControllerUtils.GetPricesFromOrder(session);
                        float TotalPrice = OrderControllerUtils.CalculateTotalPrice(currentPrices, 0);

                        CuponModel? cupon = null;
                        if (cuponCode.Length > 0) {
                            cupon = await cuponDb.GetCuponByCode(cuponCode);

                            if (cupon.ExpirationDate.ToLocalTime() <= DateTime.Now) {
                                return Results.BadRequest();
                            }

                            TotalPrice = OrderControllerUtils.CalculateTotalPrice(currentPrices,
                                cupon.DiscountPercent);
                        }

                        InsertOrderModel order = new InsertOrderModel();
                        order.ResturantAddress = restorantAddress;
                        order.UserId = (int)userId;
                        order.TotalPrice = TotalPrice;
                        order.Notes = notes;

                        orderDb.AddOrder(ControllerUtils.GetModelFromSession(session, "User").Deserialize<UserModel>(),
                            
                            OrderControllerUtils.GetDishesFromOrder(session), order);

                        if (cupon != null) {
                            cuponDb.DeleteCupon(cupon.Name);
                        }

                        OrderControllerUtils.DeleteOrderFromSession(ref session);

                        await session.CommitAsync();

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }

                }).RequireRateLimiting("fixed")
              .DisableAntiforgery();

            // stop order
            app.MapPost("/order/stop", async (HttpContext context,
                OrderDatabaseHandler db, [FromForm] int orderId) => {

                    try {

                        ISession session = context.Session;
                        int? userId = ControllerUtils.IsLoggedIn(session);
                        if (userId == null) {
                            return Results.Unauthorized();
                        }

                        string status = 
                            await db.GetOrder_CurrentStatus_ById(orderId);

                        if (!status.Equals("pending") || !status.Equals("db")) {
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
