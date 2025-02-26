using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
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

                        List<OrderModel> orders = await db.GetOrdersByUser(user.Id);
                        foreach (OrderModel order in orders)
                        {
                            order.RestorantAddress = string.Join(" - ", order.RestorantAddress.Split(';'));
                        }

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
                ControllerUtils controllerUtils, UserUtils userUtils,
                [FromForm] string notes,
                [FromForm] string cuponCode, [FromForm] string restorantAddress) => {

                    try {

                        if (string.IsNullOrWhiteSpace(restorantAddress))
                        {
                            return Results.BadRequest();
                        }

                        UserModel? user = await userUtils.GetUserModelFromAuth(context);
                        if (user == null)
                        {
                            return Results.Redirect("/login");
                        }

                        controllerUtils.GetCartItems()

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

                        context.Response.Cookies.Delete("cart");

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }

                }).RequireRateLimiting("fixed")
              .DisableAntiforgery();
            

            // stop order
            app.MapPost("/order/stop", async (HttpContext context,
                OrderDatabaseHandler db, [FromForm] string orderIdStr) => {

                    if (!int.TryParse(orderIdStr, out int orderId))
                    {
                        return Results.BadRequest();
                    }

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
