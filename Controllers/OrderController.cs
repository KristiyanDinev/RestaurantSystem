using ITStepFinalProject.Database;
using ITStepFinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ITStepFinalProject.Controllers {
    public class OrderController {


        public OrderController(WebApplication app) {

            // get front-end display of your orders
            app.MapGet("/orders", async (HttpContext context, 
                DatabaseManager db) => {

                    try {
                        ISession session = context.Session;
                    int? id = Utils.Utils.IsLoggedIn(session);
                        if (id == null) {
                            return Results.Unauthorized();
                        }

                        string FileData = await Utils.Utils.GetFileContent("/orders");

                    UserModel user = await db.GetUser((int)id);

                    Utils.Utils._handleEntryInFile(ref FileData, user, "User");
                        Utils.Utils.ApplyUserBarElement(ref FileData, user);

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


            // get current dishes about to order (not yet ordered)
            app.MapPost("/order/dish/current", async (HttpContext context,
                DatabaseManager db) =>
            {
                Dictionary<string, object> res = new Dictionary<string, object>();
                ISession session = context.Session;
                if (Utils.Utils.IsLoggedIn(session) == null)
                {
                    return res;
                }

                try
                {

                    List<DishModel> dishes = new List<DishModel>();
                    foreach (int id in GetDishesFromOrder(session))
                    {
                        DishModel dish = await db.GetDishById(id);
                        dishes.Add(dish);
                    }


                    res.Add("dishes_to_order", dishes);
                    return res;

                } catch (Exception)
                {
                    return res;
                }

            }).DisableAntiforgery().RequireRateLimiting("fixed");


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
                [FromForm] string cuponCode, [FromForm] string resturantAddress) => {

                    try {

                        ISession session = context.Session;
                        int? userId = Utils.Utils.IsLoggedIn(session);
                        if (userId == null) {
                            return Results.Unauthorized();
                        }

                        Dictionary<int, float> data = GetPricesFromOrder(session);
                        List<float> currentPrices = data.Values.ToList();
                        float TotalPrice = CalculateTotalPrice(currentPrices, 0);

                        CuponModel? cupon = null;
                        if (cuponCode.Length > 0) {
                            cupon = await db.GetCuponByCode(cuponCode);

                            if (cupon.ExpirationDate.ToLocalTime() <= DateTime.Now) {
                                return Results.BadRequest();
                            }

                            TotalPrice = CalculateTotalPrice(currentPrices,
                                cupon.DiscountPercent);
                        }

                        db.AddOrder((int)userId,
                            GetDishesFromOrder(session), 
                            notes, TotalPrice, resturantAddress);

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

            for (int i = 0; i < dishesId.Count;) {
                string dish = dishesId[i];
                // id:price
                if (dish.Length > 0 && dish.StartsWith(dishIdStr)) {
                    dishesId.RemoveAt(i);

                } else {
                    i++;
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

        private static Dictionary<int, float> GetPricesFromOrder(ISession session) {
            string? orderDishes = session.GetString("OrderDishes");
            Dictionary<int, float> dishes = new Dictionary<int, float>();
            if (orderDishes == null || orderDishes.Length == 0) {
                return dishes;
            }

            foreach (string dish in orderDishes.Split(';')) {
                if (dish.Length == 0)
                {
                    continue;
                }
                string[] parts = dish.Split(':');
                int id = int.Parse(parts[0]);
                if (!dishes.ContainsKey(id))
                {
                    dishes.Add(id, float.Parse(parts[1]));
                }
            }
            return dishes;
        }

        private static List<int> GetDishesFromOrder(ISession session)
        {
            string? orderDishes = session.GetString("OrderDishes");
            List<int> dishes = new List<int>();
            if (orderDishes == null || orderDishes.Length == 0)
            {
                return dishes;
            }

            foreach (string dish in orderDishes.Split(';'))
            {
                if (dish.Length == 0)
                {
                    continue;
                }
                string[] parts = dish.Split(':');
                dishes.Add(int.Parse(parts[0]));
            }
            return dishes;
        }

        private static float CalculateTotalPrice(List<float> prices, float discount_percent) {
            float totalPrice = prices.Sum();
            
            return totalPrice - (totalPrice * (discount_percent / 100));
        }
    }
}
