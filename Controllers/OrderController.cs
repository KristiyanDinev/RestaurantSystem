using ITStepFinalProject.Database;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ITStepFinalProject.Controllers {
    public class OrderController {


        public OrderController(WebApplication app) {

            // get front-end display of your orders
            app.MapGet("/orders", async (HttpContext context) => {

                    try {
                        ISession session = context.Session;
                        if (Utils.Utils.IsLoggedIn(session) == null) {
                            return Results.Unauthorized();
                        }

                        string FileData = await Utils.Utils.GetFileContent("/orders");

                        return Results.Content(FileData, "text/html");

                    } catch (Exception) {
                        return Results.BadRequest();
                    }
                });

            // get your own orders
            app.MapPost("/orders", async (HttpContext context,
                DatabaseManager db) => {

                    Dictionary<string, object> data =
                        new Dictionary<string, object>();

                    try {
                        ISession session = context.Session;
                        int? userId = Utils.Utils.IsLoggedIn(session);
                        if (userId == null) {
                            return data;
                        }

                        // get user's orders
                        List<OrderModel> orders = await db.GetOrdersByUser((int)userId);
                        data.Add("orders", orders);

                        return data;

                    } catch (Exception) {
                        return data;
                    }
                }).RequireRateLimiting("fixed")
                .DisableAntiforgery();

            // add dish
            app.MapPost("/order/add", async (HttpContext context, 
                DatabaseManager db, [FromForm] int dishId, 
                [FromForm] float dishPrice) => {

                    try {

                        ISession session = context.Session;
                        if (Utils.Utils.IsLoggedIn(session) == null) {
                            return Results.Unauthorized();
                        }

                        AddDishToOrder(ref session, dishId, dishPrice);

                        await session.CommitAsync();

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }
                     
            }).RequireRateLimiting("fixed")
              .DisableAntiforgery();

            // remove dish
            app.MapPost("/order/remove", async (HttpContext context,
                DatabaseManager db, [FromForm] int dishId) => {

                    try {

                        ISession session = context.Session;
                        if (Utils.Utils.IsLoggedIn(session) == null) {
                            return Results.Unauthorized();
                        }

                        RemoveDishFromOrder(ref session, dishId);
                        await session.CommitAsync();

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }

                }).RequireRateLimiting("fixed")
              .DisableAntiforgery();

            // start order
            app.MapPost("/order", async (HttpContext context,
                DatabaseManager db, [FromForm] string notes,
                [FromForm] string cuponCode, 
                [FromForm] string resturantAddress) => {

                    try {

                        ISession session = context.Session;
                        int? userId = Utils.Utils.IsLoggedIn(session);
                        if (userId == null) {
                            return Results.Unauthorized();
                        }

                        Dictionary<int, float> data = GetDishesFromOrder(session);
                        float TotalPrice = CalculateTotalPrice(data.Values.ToList(), 0);

                        CuponModel? cupon = null;
                        if (cuponCode.Length > 0) {
                            cupon = await db.GetCuponByCode(cuponCode);

                            if (cupon.ExpirationDate.ToLocalTime() <= DateTime.Now) {
                                return Results.BadRequest();
                            }

                            TotalPrice = CalculateTotalPrice(data.Values.ToList(),
                                cupon.DiscountPercent);
                        }

                        db.AddOrder((int)userId,
                            data.Keys.ToList(), notes, TotalPrice, resturantAddress);

                        if (cupon != null) {
                            db.DeleteCupon(cupon.Name);
                        }

                        session.SetString("OrderDishes", "");

                        await session.CommitAsync();

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }

                }).RequireRateLimiting("fixed")
              .DisableAntiforgery();

            // stop order
            app.MapPost("/order/stop", async (HttpContext context,
                DatabaseManager db, [FromForm] int orderId) => {

                    try {

                        ISession session = context.Session;
                        int? userId = Utils.Utils.IsLoggedIn(session);
                        if (userId == null) {
                            return Results.Unauthorized();
                        }

                        string status = 
                            await db.GetOrder_CurrentStatus_ById(orderId);

                        if (!status.Equals("pending")) {
                            return Results.BadRequest();
                        }

                        db.DeleteOrderDishes(orderId);
                        db.DeleteOrder(orderId);

                        return Results.Ok();

                    } catch (Exception) {
                        return Results.BadRequest();
                    }

                }).RequireRateLimiting("fixed")
              .DisableAntiforgery();
        }


        private static void AddDishToOrder(ref ISession session, 
            int dishId, float dishPrice) {
            string? orderDishes = session.GetString("OrderDishes");

            string newDish = dishId + ":"+ dishPrice+ ";";

            if (orderDishes == null) {
                orderDishes = newDish;

            } else {
                orderDishes += newDish;
            }

            session.SetString("OrderDishes", orderDishes);
        }

        private static void RemoveDishFromOrder(ref ISession session, int dishId) {
            string? orderDishes = session.GetString("OrderDishes");
            if (orderDishes == null || orderDishes.Length == 0) {
                return;
            }
                string dishIdStr = dishId.ToString();
                List<string> dishesId = orderDishes.Split(';').ToList();

                for (int i = 0; i < dishesId.Count; i++) {
                    string dish = dishesId[i];
                    // id:price
                    if (dish.Length > 0 && dish.StartsWith(dishIdStr)) {
                        dishesId.RemoveAt(i);
                        break;
                    }
                }

                StringBuilder stringBuilder = new StringBuilder();
                foreach (string dish in dishesId) { 
                    if (dish.Length > 0) {
                        stringBuilder.Append(dish+";");
                    }
                }

                session.SetString("OrderDishes", stringBuilder.ToString());
            
        }

        private static Dictionary<int, float> GetDishesFromOrder(ISession session) {
            string? orderDishes = session.GetString("OrderDishes");
            Dictionary<int, float> dishes = new Dictionary<int, float>();
            if (orderDishes == null || orderDishes.Length == 0) {
                return dishes;
            }

            foreach (string dish in orderDishes.Split(';')) {
                string[] parts = dish.Split(':');
                dishes.Add(int.Parse(parts[0]), float.Parse(parts[1]));
            }
            return dishes;
        }

        private static float CalculateTotalPrice(List<float> prices, float discount_percent) {
            float totalPrice = prices.Sum();
            
            return totalPrice - (totalPrice * (discount_percent / 100));
        }
    }
}
