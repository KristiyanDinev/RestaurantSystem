using ITStepFinalProject.Database.Utils;
using System.Linq;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels;

namespace ITStepFinalProject.Database.Handlers
{
    public class OrderDatabaseHandler
    {
        private static readonly string table = "Orders";
        public async void AddOrder(UserModel user, List<int> dishesId, InsertOrderModel order)
        {
            order.CurrentStatus = "db";
            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Insert(table, [order]).ToString());

            List<string> res = new List<string>();
            res.Add("UserId = "+ user.Id + " AND CurrentStatus = 'db'");

            List<object> ordersIdQ = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("Id", table)
                .Where_Set_On_Having("WHERE", res).ToString(), new OrderModel(), true);

            int orderId = ((OrderModel)ordersIdQ[0]).Id;


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
                List<string> where2 = new List<string>();
                where2.Add("Id = " + orderId);

                DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                    .Delete(table).Where_Set_On_Having("WHERE", where2).ToString());
                throw;
            }

            List<string> set = new List<string>();
            set.Add("CurrentStatus = 'pending'");

            List<string> where = new List<string>();
            where.Add("Id = "+ orderId);

            DatabaseManager._UpdateModel(table, set, where);
        }

        public async void DeleteOrder(int orderId)
        {

            List<string> where2 = new List<string>();
            where2.Add("OrderId = " + orderId);

            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Delete("OrderedDishes").Where_Set_On_Having("WHERE", where2).ToString());

            List<string> where = new List<string>();
            where.Add("Id = "+ orderId);

            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Delete(table).Where_Set_On_Having("WHERE", where).ToString());
        }

        public async Task<List<OrderModel>> GetOrdersByUser(int userId)
        {
            List<string> where = new List<string>();
            where.Add("UserId = "+userId);

            List<object> orderObj = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("*", table)
                .Where_Set_On_Having("WHERE", where).ToString(), new OrderModel(), true);

            return orderObj.Cast<OrderModel>().ToList();
        }

        public async Task<string?> GetOrder_CurrentStatus_ById(int orderId)
        {
            List<string> where = new List<string>();
            where.Add("Id = "+ orderId);

            List<object> results = await DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("CurrentStatus", table).Where_Set_On_Having("WHERE", where)
                .ToString(), new OrderModel(), true);

            return results.Count == 0 ? null : ((OrderModel)results[0]).CurrentStatus;
        }

        public async Task<List<DishModel>> GetAllDishesFromOrder(int orderId)
        {
            List<string> joinCondition = new List<string>();
            joinCondition.Add("OrderedDishes.DishId = Dishes.Id");

            List<string> where = new List<string>();
            where.Add("OrderedDishes.OrderId = "+ orderId);

            List<object> objs = await DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("Dishes.Id AS id, Dishes.Name AS name, Dishes.Price AS price, Dishes.AvrageTimeToCook AS avragetimetocook", 
                "OrderedDishes")
                .Join("Dishes", "INNER")
                .Where_Set_On_Having("ON", joinCondition)
                .Where_Set_On_Having("WHERE", where).ToString(), new DishModel(), true);

            return objs.Cast<DishModel>().ToList();
        }
    }
}
