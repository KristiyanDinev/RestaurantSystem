using ITStepFinalProject.Models;
using Npgsql;
using System.Text;

namespace ITStepFinalProject.Database.Models
{
    public class OrderDatabaseHandler
    {
        public async void AddOrder(int userId, List<int> dishesId,
            string Notes, float TotalPrice, string ResturantAddress)
        {

            string orderSql = @$"INSERT INTO Orders (CurrentStatus, Notes, TotalPrice, UserId, ResturantAddress) 
        VALUES ('db', {_handleStrings(Notes)}, 
        {TotalPrice}, {userId}, {_handleStrings(ResturantAddress)}); 
            SELECT Id FROM Orders WHERE UserId = {userId} AND CurrentStatus = 'db' LIMIT 1;";

            var orderCMD = await DatabaseCommandBuilder.BuildCommand(orderSql, null);
            using NpgsqlDataReader reader = await orderCMD.ExecuteReaderAsync();

            int? orderId = null;
            while (await reader.ReadAsync())
            {
                orderId = Convert.ToInt32(reader["id"]);
            }

            reader.Close();
            orderCMD.Connection?.Close();
            orderCMD.Dispose();

            if (orderId == null)
            {
                throw new Exception("Can't get ID of order");
            }

            StringBuilder stringBuilder =
                new StringBuilder("INSERT INTO OrderedDishes (OrderId, DishId) VALUES ");

            for (int i = 0; i < dishesId.Count - 1; i++)
            {
                stringBuilder.Append($" ({orderId}, {dishesId[i]}), ");
            }

            stringBuilder.Append($" ({orderId}, {dishesId[dishesId.Count - 1]});");

            var dishesCMD = await DatabaseCommandBuilder
                    .BuildCommand(stringBuilder.ToString(), null);
            int num = await dishesCMD.ExecuteNonQueryAsync();

            dishesCMD.Connection?.Close();
            dishesCMD.Dispose();

            if (num <= 0)
            {
                DeleteOrder((int)orderId);

                throw new Exception("Did not insert dishes");
            }

            string updateOrderStatus = "UPDATE Orders SET CurrentStatus = 'pending' WHERE CurrentStatus = 'db' AND Id = " + orderId;
            var updateCMD = await DatabaseCommandBuilder.BuildCommand(updateOrderStatus, null);
            await updateCMD.ExecuteNonQueryAsync();

            updateCMD.Connection?.Close();
            updateCMD.Dispose();
        }

        public async void DeleteOrder(int orderId)
        {
            string orderSql = $"DELETE FROM Orders WHERE Id = {orderId};";

            var orderCMD = await DatabaseCommandBuilder.BuildCommand(orderSql, null);
            int num = await orderCMD.ExecuteNonQueryAsync();

            orderCMD.Connection?.Close();
            orderCMD.Dispose();

            if (num <= 0)
            {
                throw new Exception("Can't delete order");
            }
        }

        public async void DeleteOrderDishes(int orderId)
        {
            string orderSql = $"DELETE FROM OrderedDishes WHERE OrderId = {orderId};";

            var orderCMD = await DatabaseCommandBuilder.BuildCommand(orderSql, null);
            int num = await orderCMD.ExecuteNonQueryAsync();

            orderCMD.Connection?.Close();
            orderCMD.Dispose();

            if (num <= 0)
            {
                throw new Exception("Can't delete order");
            }
        }

        public async Task<List<OrderModel>> GetOrdersByUser(int id)
        {
            string sql = "SELECT * FROM Orders WHERE UserId = " + id + @"
                JOIN OrderedDishes ON OrderedDishes.OrderId = Orders.Id
                JOIN Dishes ON OrderedDishes.DishId = Dishes.Id";

            var getOrdersCMD = await DatabaseCommandBuilder.BuildCommand(sql, null);
            using NpgsqlDataReader reader = await getOrdersCMD.ExecuteReaderAsync();

            List<OrderModel> orders = new List<OrderModel>();

            OrderModel orderModel = new OrderModel();
            while (await reader.ReadAsync())
            {

                int lookingAtOrderId = Convert.ToInt32(reader["id"]);

                if (orderModel.Id != lookingAtOrderId)
                {
                    orders.Add(orderModel);
                    orderModel.Dishes.Clear();
                }

                orderModel = ConvertToOrder(reader, orderModel);
            }
            reader.Close();
            getOrdersCMD.Connection?.Close();
            getOrdersCMD.Dispose();
            return orders;
        }

        public async Task<string> GetOrder_CurrentStatus_ById(int id)
        {
            string sql = "SELECT CurrentStatus FROM Orders WHERE Id = " + id;

            var cmd = await DatabaseCommandBuilder.BuildCommand(sql, null);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            string? CurrentStatus = null;
            while (await reader.ReadAsync())
            {
                CurrentStatus = Convert.ToString(reader["currentstatus"]);
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();

            if (CurrentStatus == null)
            {
                throw new Exception("No such order");
            }

            return CurrentStatus;
        }


    }
}
