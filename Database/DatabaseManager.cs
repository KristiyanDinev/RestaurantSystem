using ITStepFinalProject.Models;
using Npgsql;
using NpgsqlTypes;
using System.Net.Http.Headers;
using System.Text;

namespace ITStepFinalProject.Database {
    public class DatabaseManager {
        // Username, password and email are required

        /*
         * 
         * insert into Dishes (Name, Price, Ingredients, Grams, Type_Of_Dish, IsAvailable, AvrageTimeToCook)
values
('Salad 1', 10.99, 'some stuff in it and this and that', 440, 'salad', true, '3'),
('Salad 2', 12.99, 'some stuff in it and this and that', 500, 'salad', false, '2'),
('Drink 1', 13.99, 'some stuff in it and this and that', 100, 'drink', true, '1'),
('Drink 2', 10.00, 'some stuff in it and this and that', 100, 'drink', false, '1'),
('Appetizer 1', 10.09, 'some stuff in it and this and that', 440, 'appetizers', true, '3'),
('Appetizer 2', 20.19, 'some stuff in it and this and that', 140, 'appetizers', false, '4'),
('Desserts 1', 23.19, 'some stuff in it and this and that', 140, 'desserts', true, '1'),
('Desserts 2', 30.20, 'some stuff in it and this and that', 120, 'desserts', false, '4'),
('Dishes 1', 21.19, 'some stuff in it and this and that', 142, 'dishes', true, '2'),
('Dishes 2', 30.22, 'some stuff in it and this and that', 120, 'dishes', false, '3')
         * 
         * 
         */

        private static readonly string _hashingSlat = "D6RTYFUYGIBUNOI";

        public static async void Setup() {
            string sql = """
                CREATE TABLE IF NOT EXISTS Users (
                    Id SERIAL PRIMARY KEY,
                    Username VARCHAR(255) NOT NULL UNIQUE,
                    Password VARCHAR(64) NOT NULL,
                    Image TEXT,
                    Address TEXT NOT NULL,
                    PhoneNumber VARCHAR(15),
                    Email VARCHAR(50) NOT NULL UNIQUE,
                    Notes VARCHAR(255)
                );

                CREATE TABLE IF NOT EXISTS Dishes (
                    Id SERIAL PRIMARY KEY,
                    Name VARCHAR(100) NOT NULL,
                    Price NUMERIC(10, 2) NOT NULL,
                    Ingredients TEXT NOT NULL,
                    Grams SMALLINT NOT NULL,
                    Type_Of_Dish VARCHAR(50) NOT NULL,
                    IsAvailable BOOLEAN NOT NULL DEFAULT TRUE,
                    AvrageTimeToCook VARCHAR(20) NOT NULL,
                    Image TEXT
                );


                CREATE TABLE IF NOT EXISTS Orders (
                    Id SERIAL PRIMARY KEY,
                    CurrentStatus VARCHAR(100) NOT NULL,
                    Notes VARCHAR(255),
                    OrderedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                    UserId INT REFERENCES Users(Id),
                    ResturantAddress TEXT NOT NULL,
                    TotalPrice NUMERIC(10, 2) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS OrderedDishes (
                    OrderId INT REFERENCES Orders(Id),
                    DishId INT REFERENCES Dishes(Id)
                );

                CREATE TABLE IF NOT EXISTS ResturantAddressAvrageDeliverTime (
                    Address TEXT NOT NULL,
                    TimeFrame TEXT NOT NULL,
                    AvrageDeliverTime VARCHAR(20) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Reservations (
                    Id SERIAL PRIMARY KEY,
                    ReservatorId INT REFERENCES Users(Id),
                    Amount_Of_Adults SMALLINT NOT NULL DEFAULT 0,
                    Amount_Of_Children SMALLINT NOT NULL DEFAULT 0,
                    At_Date TIMESTAMP WITH TIME ZONE NOT NULL,
                    Created_At TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
                );

                CREATE TABLE IF NOT EXISTS Cupons (
                    CuponCode VARCHAR(25) NOT NULL UNIQUE,
                    DiscountPercent NUMERIC(10, 2) NOT NULL,
                    ExpirationDate TIMESTAMP WITH TIME ZONE NOT NULL,
                    Name VARCHAR(100) NOT NULL
                );

                """;


            var cmd = await DatabaseCommandBuilder.BuildCommand(sql, null);
            await cmd.ExecuteNonQueryAsync();
            cmd.Dispose();
        }

        public async Task<UserModel> GetUser(int id) {
            List<NpgsqlParameter> npgsqlParameters = new List<NpgsqlParameter>();

            NpgsqlParameter idArg = new NpgsqlParameter("@Id", NpgsqlDbType.Integer);
            idArg.Value = id;

            npgsqlParameters.Add(idArg);

            string sql = "SELECT * FROM Users WHERE Id = @Id";


            var cmd = await DatabaseCommandBuilder.BuildCommand(
                sql, npgsqlParameters);
            cmd.Prepare();
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            UserModel model = new UserModel();
            while (await reader.ReadAsync()) {
                model = ConvertToUser(reader);
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();
            return model;
        }

        public async Task<UserModel> RegisterUser(UserModel model, string password) {

            string email = _handleStrings(model.Email);
            string hashedPass = _hashString(password);

            string sql = @$"INSERT INTO Users 
    (Username, Password, Image, Address, PhoneNumber, Email, Notes) VALUES 
    ({_handleStrings(model.Username)}, '{hashedPass}', {_handleStrings(model.Image)}, 
    {_handleStrings(model.Address)}, {_handleStrings(model.PhoneNumber)}, 
    {email}, {_handleStrings(model.Notes)}); 
        SELECT * FROM Users WHERE Email = {email} AND Password = '{hashedPass}';
";

            var cmd = await DatabaseCommandBuilder.BuildCommand(sql, null);
            
            
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            UserModel user = new UserModel();
            while (await reader.ReadAsync()) {
                user = ConvertToUser(reader);
            }

            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();

            return user;
        }

        public async Task<UserModel> LoginUser(string email, string password) {

            string sql = $"SELECT * FROM Users WHERE Email = {_handleStrings(email)} AND Password = '{_hashString(password)}' LIMIT 1";
          
            var cmd = await DatabaseCommandBuilder.BuildCommand(sql, null);
            cmd.Prepare();
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) {
                UserModel user = ConvertToUser(reader);
                reader.Close();
                cmd.Connection?.Close();
                cmd.Dispose();
                return user;
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();
            throw new Exception("Didn't login");
        }

        public async Task<List<DishModel>> GetDishes(string type) {
            string sql = $"SELECT * FROM Dishes WHERE Type_Of_Dish = {_handleStrings(type)}";

            List<DishModel> dishModels = new List<DishModel>();
            var cmd = await DatabaseCommandBuilder.BuildCommand(
                sql, null);
            cmd.Prepare();
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) {

                dishModels.Add(ConvertToDish(reader));
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();
            return dishModels;
        }

        public async Task<DishModel> GetDishById(int id) {
            string sql = "SELECT * FROM Dishes WHERE id = "+id;

            var cmd = await DatabaseCommandBuilder.BuildCommand(
                sql, null);
            cmd.Prepare();
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            DishModel dish = new DishModel();
            while (await reader.ReadAsync()) {
                dish = ConvertToDish(reader);
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();
            return dish;
        }


        public async void AddOrder(int userId, List<int> dishesId,
            string Notes, float TotalPrice, string ResturantAddress) {

            string orderSql = @$"INSERT INTO Orders (CurrentStatus, Notes, TotalPrice, UserId, ResturantAddress) 
        VALUES ('db', {_handleStrings(Notes)}, 
        {TotalPrice}, {userId}, {_handleStrings(ResturantAddress)}); 
            SELECT Id FROM Orders WHERE UserId = {userId} AND CurrentStatus = 'db' LIMIT 1;";

            var orderCMD = await DatabaseCommandBuilder.BuildCommand(orderSql, null);
            using NpgsqlDataReader reader = await orderCMD.ExecuteReaderAsync();

            int? orderId = null;
            while (await reader.ReadAsync()) {
                orderId = Convert.ToInt32(reader["id"]);
            }

            reader.Close();
            orderCMD.Connection?.Close();
            orderCMD.Dispose();

            if (orderId == null) {
                throw new Exception("Can't get ID of order");
            }

            StringBuilder stringBuilder = 
                new StringBuilder("INSERT INTO OrderedDishes (OrderId, DishId) VALUES ");

            for (int i = 0; i < dishesId.Count - 1; i++) {
                stringBuilder.Append($" ({orderId}, {dishesId[i]}), ");
            }

            stringBuilder.Append($" ({orderId}, {dishesId[dishesId.Count - 1]});");

            var dishesCMD = await DatabaseCommandBuilder
                    .BuildCommand(stringBuilder.ToString(), null);
            int num = await dishesCMD.ExecuteNonQueryAsync();

            dishesCMD.Connection?.Close();
            dishesCMD.Dispose();

            if (num <= 0) {
                DeleteOrder((int)orderId);

                throw new Exception("Did not insert dishes");
            }

            string updateOrderStatus = "UPDATE Orders SET CurrentStatus = 'pending' WHERE CurrentStatus = 'db' AND Id = " + orderId;
            var updateCMD = await DatabaseCommandBuilder.BuildCommand(updateOrderStatus, null);
             await updateCMD.ExecuteNonQueryAsync();

            updateCMD.Connection?.Close();
            updateCMD.Dispose();
        }

        public async void DeleteOrder(int orderId) {
            string orderSql = $"DELETE FROM Orders WHERE Id = {orderId};";

            var orderCMD = await DatabaseCommandBuilder.BuildCommand(orderSql, null);
            int num = await orderCMD.ExecuteNonQueryAsync();

            orderCMD.Connection?.Close();
            orderCMD.Dispose();

            if (num <= 0) {
                throw new Exception("Can't delete order");
            }
        }

        public async void DeleteOrderDishes(int orderId) {
            string orderSql = $"DELETE FROM OrderedDishes WHERE OrderId = {orderId};";

            var orderCMD = await DatabaseCommandBuilder.BuildCommand(orderSql, null);
            int num = await orderCMD.ExecuteNonQueryAsync();

            orderCMD.Connection?.Close();
            orderCMD.Dispose();

            if (num <= 0) {
                throw new Exception("Can't delete order");
            }
        }

        public async Task<List<OrderModel>> GetOrdersByUser(int id) {
            string sql = "SELECT * FROM Orders WHERE UserId = "+id+ @"
                JOIN OrderedDishes ON OrderedDishes.OrderId = Orders.Id
                JOIN Dishes ON OrderedDishes.DishId = Dishes.Id";

            var getOrdersCMD = await DatabaseCommandBuilder.BuildCommand(sql, null);
            using NpgsqlDataReader reader = await getOrdersCMD.ExecuteReaderAsync();

            List<OrderModel> orders = new List<OrderModel>();

            OrderModel orderModel = new OrderModel();
            while (await reader.ReadAsync()) {

                int lookingAtOrderId = Convert.ToInt32(reader["id"]);

                if (orderModel.Id != lookingAtOrderId) {
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

        public async Task<string> GetOrder_CurrentStatus_ById(int id) {
            string sql = "SELECT CurrentStatus FROM Orders WHERE Id = " + id;

            var cmd = await DatabaseCommandBuilder.BuildCommand(sql, null);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            string? CurrentStatus = null;
            while (await reader.ReadAsync()) {
                CurrentStatus = Convert.ToString(reader["currentstatus"]);
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();

            if (CurrentStatus == null) {
                throw new Exception("No such order");
            }

            return CurrentStatus;
        }

        public async void DeleteCupon(string cuponCode) {
            string cuponSql = $"DELETE FROM Cupons WHERE CuponCode = {_handleStrings(cuponCode)};";

            var cuponCMD = await DatabaseCommandBuilder.BuildCommand(cuponSql, null);
            int num = await cuponCMD.ExecuteNonQueryAsync();

            cuponCMD.Connection?.Close();
            cuponCMD.Dispose();

            if (num <= 0) {
                throw new Exception("Can't delete cupon");
            }
        }

        public async Task<CuponModel> GetCuponByCode(string cuponCode) {
            string sql = $"SELECT * FROM Cupons WHERE CuponCode = {_handleStrings(cuponCode)}";

            var cmd = await DatabaseCommandBuilder.BuildCommand(sql, null);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            CuponModel model = new CuponModel();
            while (await reader.ReadAsync()) {
                model = ConvertToCupon(reader);
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();

            return model;
        }
       

        public static string _hashString(string str) {
            str += _hashingSlat;
            return Encoding.ASCII.GetString(
                Program.hashing?.ComputeHash(Encoding.ASCII.GetBytes(str)) ?? []);
        }

        private static string _handleStrings(string? str) {
            return str == null || str.Replace(" ", "").Length == 0 ? 
                "null" : "'" + str.Replace("'", "''") + "'";
        }

        private static DishModel ConvertToDish(NpgsqlDataReader reader) {
            DishModel dish = new DishModel();
            dish.Id = Convert.ToInt32(reader["id"]);
            dish.Name = Convert.ToString(reader["name"]);
            dish.Grams = Convert.ToInt32(reader["grams"]);
            dish.AvrageTimeToCook = Convert.ToString(reader["avragetimetocook"]);
            dish.Type_Of_Dish = Convert.ToString(reader["type_of_dish"]);
            dish.Price = float.Parse(Convert.ToString(reader["price"]));
            dish.Image = Convert.ToString(reader["image"]);
            dish.IsAvailable = bool.Parse(Convert.ToString(reader["isavailable"]));
            dish.Ingredients = Convert.ToString(reader["ingredients"]);
            return dish;
        }

        private static UserModel ConvertToUser(NpgsqlDataReader reader) {
            UserModel user = new UserModel();
            user.Username = Convert.ToString(reader["username"]);
            user.Image = Convert.ToString(reader["image"]);
            user.Address = Convert.ToString(reader["address"]);
            user.PhoneNumber = Convert.ToString(reader["phonenumber"]);
            user.Email = Convert.ToString(reader["email"]);
            user.Notes = Convert.ToString(reader["notes"]);
            user.Id = Convert.ToInt32(reader["id"]);
            return user;
        }

        private static OrderModel ConvertToOrder(NpgsqlDataReader reader, 
               OrderModel orderModel) {

            if (orderModel.Dishes.Count == 0) {
                orderModel.Id = Convert.ToInt32(reader["id"]);
                orderModel.CurrentStatus = Convert.ToString(reader["currentstatus"]);
                orderModel.ResturantAddress = Convert.ToString(reader["resturantaddress"]);
                orderModel.Notes = Convert.ToString(reader["notes"] ?? "");
                orderModel.OrderedAt = DateTime.Parse(Convert.ToString(reader["orderedat"]));
                orderModel.UserId = Convert.ToInt32(reader["userid"]);
                orderModel.TotalPrice = float.Parse(Convert.ToString(reader["totalprice"]));
            }

            orderModel.Dishes.Add(ConvertToDish(reader));
            return orderModel;
        }

        private static CuponModel ConvertToCupon(NpgsqlDataReader reader) { 
            CuponModel model = new CuponModel();

            model.CuponCode = Convert.ToString(reader["cuponcode"]);
            model.DiscountPercent = float.Parse(Convert.ToString(reader["discountpercent"]));
            model.ExpirationDate = DateTime.Parse(Convert.ToString(reader["expirationdate"]));

            return model;
        }
    }
}
