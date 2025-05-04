using RestaurantSystem.Models.DatabaseModels;
using System.Text;

namespace RestaurantSystem.Utils.Controller
{
    public class OrderUtils
    {
        public static void AddDishToOrder(ref ISession session,
            int dishId, float dishPrice)
        {
            string? orderDishes = session.GetString("OrderDishes");

            string newDish = dishId + ":" + dishPrice + ";";

            if (orderDishes == null)
            {
                orderDishes = newDish;

            }
            else
            {
                orderDishes += newDish;
            }

            session.SetString("OrderDishes", orderDishes);
        }

        public static void RemoveOneDishByIDFromOrder(ref ISession session, int dishId)
        {
            string? orderDishes = session.GetString("OrderDishes");
            if (orderDishes == null || orderDishes.Length == 0)
            {
                return;
            }
            string dishIdStr = dishId.ToString();
            List<string> dishesId = orderDishes.Split(';').ToList();

            for (int i = 0; i < dishesId.Count;)
            {
                string dish = dishesId[i];
                // id:price
                if (dish.Length > 0 && dish.StartsWith(dishIdStr))
                {
                    dishesId.RemoveAt(i);
                    break;
                }
                else
                {
                    i++;
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (string dish in dishesId)
            {
                if (dish.Length > 0)
                {
                    stringBuilder.Append(dish + ";");
                }
            }

            session.SetString("OrderDishes", stringBuilder.ToString());
        }

        public static List<float> GetPricesFromOrder(ISession session)
        {
            string? orderDishes = session.GetString("OrderDishes");
            List<float> prices = new List<float>();
            if (orderDishes == null || orderDishes.Length == 0)
            {
                return prices;
            }

            foreach (string dish in orderDishes.Split(';'))
            {
                if (dish.Length == 0)
                {
                    continue;
                }
                string[] parts = dish.Split(':');
                //int id = int.Parse(parts[0]);
                prices.Add(float.Parse(parts[1]));
            }
            return prices;
        }

        public static List<int> GetDishesFromOrder(ISession session)
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

        public static decimal CalculateTotalPrice(List<DishModel> dishes, decimal discount_percent)
        {
            decimal totalPrice = 0;
            foreach (DishModel dish in dishes)
            {
                totalPrice += dish.Price;
            }

            if (discount_percent == 0)
            {
                return totalPrice;

            } else
            {
                return totalPrice - totalPrice * (discount_percent / 100);
            }
        }

        public static void DeleteOrderFromSession(ref ISession session)
        {
            session.SetString("OrderDishes", "");
        }
    }
}
