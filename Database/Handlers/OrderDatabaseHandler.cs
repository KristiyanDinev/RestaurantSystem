﻿using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels;
using ITStepFinalProject.Models.WebModels;
using ITStepFinalProject.Utils.Controller;
using Npgsql;
using System.Linq;

namespace ITStepFinalProject.Database.Handlers
{
    public class OrderDatabaseHandler
    {
        private static readonly string table = "Orders";
        private static readonly string tableRestorant = "Restorant";
        private static readonly string tableOrderedDishes = "OrderedDishes";
        private static readonly string tableTimeTable = "TimeTable";
        private static readonly string tableDishes = "Dishes";

        public async void AddOrder(int userId, List<int> dishesId, InsertOrderModel order, 
            ControllerUtils utils)
        {
            order.CurrentStatus = utils.DBStatus;

            NpgsqlCommand cmd = await DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Insert(table, [order]).ToString(), true) ?? throw new Exception();

            int? orderId = null;
            try
            {
                ResultSqlQuery ordersIdQ = await DatabaseManager._ExecuteQuery(
                    new SqlBuilder().Select("\"Id\"", table)
                    .ConditionKeyword("WHERE")
                    .BuildCondition("UserId", userId, "=", "AND")
                    .BuildCondition("CurrentStatus", "'"+utils.DBStatus+"'")
                    .ToString(), new OrderModel(), false, cmd.Connection);

                orderId = ((OrderModel)ordersIdQ.Models[0]).Id;


                List<OrderedDishesModel> orderedDishes = new List<OrderedDishesModel>();
                foreach (int dishId in dishesId)
                {
                    orderedDishes.Add(new OrderedDishesModel((int)orderId, dishId));
                }

            
                await DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                    .Insert("OrderedDishes", 
                    orderedDishes.Cast<object>().ToList()).ToString(), false, cmd.Connection);

                cmd.Transaction.Commit();

            } catch (Exception)
            {
                cmd.Transaction.Rollback();
                throw;
            }

            UpdateOrderCurrentStatusById((int)orderId, utils.PendingStatus);
        }

        public async void DeleteOrder(int orderId, UserModel user)
        {
            await DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Delete(tableOrderedDishes)
                .ConditionKeyword("WHERE")
                .BuildCondition("OrderId", orderId)
                .ToString());

            await DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Delete(table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Id", orderId, "=", "AND ")
                .BuildCondition("UserId", user.Id)
                .ToString());
        }

        public async void UpdateOrderCurrentStatusById(int orderId, string newStatus)
        {
            await DatabaseManager._ExecuteNonQuery(
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
            ResultSqlQuery orderObj = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("*", table)
                .Join(tableRestorant, "INNER")
                .ConditionKeyword("ON")
                .BuildCondition(tableRestorant+".Id", '"'+table + "\".\"RestorantId\"")

                .ConditionKeyword("WHERE")
                .BuildCondition("UserId", userId)
                .ToString(), new DisplayOrderModel());

            return orderObj.Models.Cast<DisplayOrderModel>().ToList();
        }

        public async Task<string?> GetOrder_CurrentStatus_ById(int orderId)
        {

            ResultSqlQuery results = await DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("\"CurrentStatus\"", table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Id", orderId)
                .ToString(), new OrderModel());

            return results.Models.Count == 0 ? null : ((OrderModel)results.Models[0]).CurrentStatus;
        }

        public async Task<List<DishModel>> GetAllDishesFromOrder(int orderId)
        {
            ResultSqlQuery objs = await DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("*", tableOrderedDishes)
                .Join("Dishes", "INNER")

                .ConditionKeyword("ON")
                .BuildCondition(tableOrderedDishes + ".DishId", "\"Dishes\".\"Id\"")

                .ConditionKeyword("WHERE")
                .BuildCondition(tableOrderedDishes + ".OrderId", orderId)

                .ToString(), new DishModel());

            return objs.Models.Cast<DishModel>().ToList();
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

            ResultSqlQuery objs = await DatabaseManager._ExecuteQuery(sqlBuilder
                .ToString(), new TimeTableJoinRestorantModel());

            return objs.Models.Cast<TimeTableJoinRestorantModel>().ToList();
        }

        public async Task<TimeTableJoinRestorantModel> GetRestorantAddressById(int id)
        {
            ResultSqlQuery objs = await DatabaseManager._ExecuteQuery(
                new SqlBuilder()
                .Select("*", tableTimeTable)
                .Join(tableRestorant, "INNER")
                .ConditionKeyword("ON")
                .BuildCondition(tableRestorant + ".Id", '"' + tableTimeTable + "\".\"RestorantId\"")

                .ConditionKeyword("WHERE")
                .BuildCondition("RestorantId", id)
                .ToString(), new TimeTableJoinRestorantModel());

            return (TimeTableJoinRestorantModel)objs.Models[0];
        }

        public async Task<List<TimeTableJoinRestorantModel>> GetRestorantAddressesWhere_DishId_IsAvailable(int dishId)
        {
            ResultSqlQuery objs = await DatabaseManager._ExecuteQuery(
                new SqlBuilder()
                .Select("*", tableTimeTable)

                .Join(tableRestorant, "INNER")
                .ConditionKeyword("ON")
                .BuildCondition(tableRestorant + ".Id", '"' + tableTimeTable + "\".\"RestorantId\"")

                .Join(tableDishes, "INNER")
                .ConditionKeyword("ON")
                .BuildCondition(tableDishes + ".RestorantId",   '"' +tableRestorant + "\".\"Id\"")


                .ConditionKeyword("WHERE")
                .BuildCondition(tableDishes + ".Id", dishId, "=", "AND ")
                .BuildCondition(tableDishes + ".IsAvailable", "'1'")
                .ToString(), new TimeTableJoinRestorantModel());

            return objs.Models.Cast<TimeTableJoinRestorantModel>().ToList();
        }
    }
}
