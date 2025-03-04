using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels;
using ITStepFinalProject.Models.WebModels;
using ITStepFinalProject.Utils.Controller;

namespace ITStepFinalProject.Database.Handlers
{
    public class OrderDatabaseHandler
    {
        private static readonly string table = "Orders";
        private static readonly string tableRestorant = "Restorant";
        private static readonly string tableOrderedDishes = "OrderedDishes";
        private static readonly string tableTimeTable = "TimeTable";

        public async void AddOrder(UserModel user, List<int> dishesId, InsertOrderModel order, 
            ControllerUtils utils)
        {
            order.CurrentStatus = utils.DBStatus;
            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Insert(table, [order]).ToString());

            List<object> ordersIdQ = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("\"Id\"", table)
                .ConditionKeyword("WHERE")
                .BuildCondition("UserId", user.Id, "=", "AND")
                .BuildCondition("CurrentStatus", "'"+utils.DBStatus+"'")
                .ToString(), new OrderModel(), true);

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
                DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                    .Delete(table)
                    .ConditionKeyword("WHERE")
                    .BuildCondition("Id", orderId)
                    .ToString());
                throw;
            }

            UpdateOrderCurrentStatusById(orderId, utils.PendingStatus);
        }

        public async void DeleteOrder(int orderId)
        {
            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Delete(tableOrderedDishes)
                .ConditionKeyword("WHERE")
                .BuildCondition("OrderId", orderId)
                .ToString());

            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Delete(table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Id", orderId)
                .ToString());
        }

        public async void UpdateOrderCurrentStatusById(int orderId, string newStatus)
        {
            DatabaseManager._ExecuteNonQuery(
                new SqlBuilder()
                .Update(table)

                .ConditionKeyword("SET")
                .BuildCondition("CurrentStatus", "'"+newStatus+"'")

                .ConditionKeyword("WHERE")
                .BuildCondition("Id", orderId)
                .ToString()
                );
        }

        public async Task<List<DisplayOrderModel>> GetOrdersByUser(int userId)
        {
            List<object> orderObj = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("*", table)
                .Join(tableRestorant, "INNER")
                .ConditionKeyword("ON")
                .BuildCondition(tableRestorant+".Id", '"'+table + "\".\"RestorantId\"")

                .ConditionKeyword("WHERE")
                .BuildCondition("UserId", userId)
                .ToString(), new DisplayOrderModel(), true);

            return orderObj.Cast<DisplayOrderModel>().ToList();
        }

        public async Task<string?> GetOrder_CurrentStatus_ById(int orderId)
        {

            List<object> results = await DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("\"CurrentStatus\"", table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Id", orderId)
                .ToString(), new OrderModel(), true);

            return results.Count == 0 ? null : ((OrderModel)results[0]).CurrentStatus;
        }

        public async Task<List<DishModel>> GetAllDishesFromOrder(int orderId)
        {
            List<object> objs = await DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("*", tableOrderedDishes)
                .Join("Dishes", "INNER")

                .ConditionKeyword("ON")
                .BuildCondition(tableOrderedDishes + ".DishId", "\"Dishes\".\"Id\"")

                .ConditionKeyword("WHERE")
                .BuildCondition(tableOrderedDishes + ".OrderId", orderId)

                .ToString(), new DishModel(), true);

            return objs.Cast<DishModel>().ToList();
        }


        public async Task<List<TimeTableJoinRestorantModel>> GetRestorantsAddressesForUser(UserModel user)
        {
            string city = ValueHandler.Strings(user.City);
            string country = ValueHandler.Strings(user.Country);

            SqlBuilder sqlBuilder = new SqlBuilder()
                .Select("*", tableTimeTable)
                .Join(tableRestorant, "INNER")
                .ConditionKeyword("ON")
                .BuildCondition(tableRestorant + ".Id", '"' + tableTimeTable + "\".\"RestorantId\"")

                .ConditionKeyword("WHERE")
                .BuildCondition("DoDelivery", "'1'", "=", "AND")
                .BuildCondition("UserCity", city, "=", "AND")
                .BuildCondition("RestorantCity", city, "=", "AND")
                .BuildCondition("UserCountry", country, "=", "AND")
                .BuildCondition("RestorantCountry", country, "=", "AND")
                .BuildCondition("UserAddress", ValueHandler.Strings('%' + user.Address + '%'), "LIKE", "AND");

            string state = ValueHandler.Strings(user.State);
            if (state.Equals("null"))
            {
                sqlBuilder.BuildCondition("UserState", "NULL", "IS", "AND");
                sqlBuilder.BuildCondition("RestorantState", "NULL", "IS");

            } else
            {
                sqlBuilder.BuildCondition("UserState", state, "=", "AND");
                sqlBuilder.BuildCondition("RestorantState", state, "=");
            }

            List<object> objs = await DatabaseManager._ExecuteQuery(sqlBuilder
                .ToString(), new TimeTableJoinRestorantModel(), true);

            return objs.Cast<TimeTableJoinRestorantModel>().ToList();
        }
    }
}
