﻿using ITStepFinalProject.Models;
using ITStepFinalProject.Database.Utils;

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
            res.Add("UserId = "+ user.Id + " AND ");
            res.Add("CurrentStatus = 'db'");

            List<object> ordersIdQ = await DatabaseManager._ExecuteQuery(
                new SqlBuilder().Select("Id", table)
                .Where_Set_On_Having("WHERE", res).ToString(), order, true);

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
            List<string> where = new List<string>();
            where.Add("Id = "+ orderId);

            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Delete(table).Where_Set_On_Having("WHERE", where).ToString());

            List<string> where2 = new List<string>();
            where2.Add("OrderId = " + orderId);

            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Delete("OrderedDishes").Where_Set_On_Having("WHERE", where2).ToString());
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

        public async Task<string> GetOrder_CurrentStatus_ById(int orderId)
        {
            List<string> where = new List<string>();
            where.Add("Id = "+ orderId);

            List<object> results = await DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("CurrentStatus", table).Where_Set_On_Having("WHERE", where)
                .ToString(), new OrderModel(), true);

            return ((OrderModel)results[0]).CurrentStatus;
        }
    }
}
