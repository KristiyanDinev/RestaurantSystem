using ITStepFinalProject.Models;
using ITStepFinalProject.Database.Utils;
using Npgsql;
using System.Text;

namespace ITStepFinalProject.Database.Models
{
    public class OrderDatabaseHandler
    {
        public async void AddOrder(UserModel user, List<int> dishesId, OrderModel order)
        {
            order.CurrentStatus = "db";
            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Insert("Orders", [order]).ToString());

            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("UserId", user.Id);
            res.Add("CurrentStatus", "db");

            List<object> ordersIdQ = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("Id", "Orders")
                .Where_Set("WHERE", res).ToString(), order, true);

            int orderId = int.Parse(Convert.ToString(ordersIdQ[0]));


            List<OrderedDishesModel> orderedDishes = new List<OrderedDishesModel>();
            foreach (int dishId in dishesId)
            {
                orderedDishes.Add(new OrderedDishesModel(orderId, dishId));
            }

            try
            {
                DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                    .Insert("OrderedDishes", 
                    orderedDishes.Cast<object>().ToList()).ToString());
            } catch (Exception)
            {
                DeleteOrder(orderId);
                throw;
            }

            Dictionary<string, object> set = new Dictionary<string, object>();
            set.Add("CurrentStatus", "pending");

            Dictionary<string, object> where = new Dictionary<string, object>();
            where.Add("Id", orderId);

            DatabaseManager._UpdateModel("Orders", set, where);
        }

        public async void DeleteOrder(int orderId)
        {
            Dictionary<string, object> where = new Dictionary<string, object>();
            where.Add("Id", orderId);

            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Delete("Orders").Where_Set("WHERE", where).ToString());
        }


        // maybe delete DeleteOrderDishes
        public async void DeleteOrderDishes(int orderId)
        {
            string orderSql = $"DELETE FROM OrderedDishes WHERE OrderId = {orderId};";

            var orderCMD = await DatabaseCommandBuilder.BuildCommand(orderSql);
            int num = await orderCMD.ExecuteNonQueryAsync();

            orderCMD.Connection?.Close();
            orderCMD.Dispose();

            if (num <= 0)
            {
                throw new Exception("Can't delete order");
            }
        }

        public async Task<List<OrderModel>> GetOrdersByUser(int userId)
        {
            Dictionary<string, object> where = new Dictionary<string, object>();
            where.Add("UserId", userId);

            List<object> orderObj = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("*", "Orders")
                .Where_Set("WHERE", where).ToString(), new OrderModel(), true);

            return orderObj.Cast<OrderModel>().ToList();
        }

        public async Task<string> GetOrder_CurrentStatus_ById(int orderId)
        {
            Dictionary<string, object> where = new Dictionary<string, object>();
            where.Add("Id", orderId);

            List<object> results = await DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("CurrentStatus", "Orders").Where_Set("WHERE", where)
                .ToString(), new OrderModel(), true);

            return Convert.ToString(results[0]) ?? "";
        }
    }
}
