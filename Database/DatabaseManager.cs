using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using Npgsql;
using NpgsqlTypes;
using System.Reflection;
using System.Text;

namespace ITStepFinalProject.Database {
    public class DatabaseManager {
        // Username, password and email are required

        /*
         * 
         * insert into Dishes (Name, Price, Ingredients, Grams, Type_Of_Dish, IsAvailable, AvrageTimeToCook, Image)
values
('Salad 1', 10.99, 'some stuff in it and this and that', 440, 'salad', true, '3', '/images/salad/1.png'),
('Salad 2', 12.99, 'some stuff in it and this and that', 500, 'salad', false, '2', null),
('Drink 1', 13.99, 'some stuff in it and this and that', 100, 'drink', true, '1', null),
('Drink 2', 10.00, 'some stuff in it and this and that', 100, 'drink', false, '1', null),
('Appetizer 1', 10.09, 'some stuff in it and this and that', 440, 'appetizers', true, '3', null),
('Appetizer 2', 20.19, 'some stuff in it and this and that', 140, 'appetizers', false, '4', null),
('Desserts 1', 23.19, 'some stuff in it and this and that', 140, 'desserts', true, '1', null),
('Desserts 2', 30.20, 'some stuff in it and this and that', 120, 'desserts', false, '4', null),
('Dishes 1', 21.19, 'some stuff in it and this and that', 142, 'dishes', true, '2', null),
('Dishes 2', 30.22, 'some stuff in it and this and that', 120, 'dishes', false, '3', null)
         * 
         * 
         */

        private static readonly string _hashingSlat = "D6RTYFUYGIBUNOI";

        public static async void Setup() {
            string sql = """
                CREATE TABLE IF NOT EXISTS Users (
                    __Id SERIAL PRIMARY KEY,
                    Username VARCHAR(100) NOT NULL UNIQUE,
                    _Password VARCHAR(64) NOT NULL,
                    Image TEXT,
                    FullAddress TEXT NOT NULL,
                    PhoneNumber VARCHAR(15),
                    _Email VARCHAR(50) NOT NULL UNIQUE,
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

            string sql = "SELECT * FROM Users WHERE __Id = @Id";


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

            string email = _handleStrings(model._Email);
            string hashedPass = _hashString(password);

            string sql = @$"INSERT INTO Users 
    (Username, Password, Image, Address, City, Country, PhoneNumber, Email, Notes) VALUES 
    ({_handleStrings(model.Username)}, '{hashedPass}', {_handleStrings(model.Image)}, 
    {_handleStrings(model.FullAddress)}, {_handleStrings(model.City)}, {_handleStrings(model.Country)}
    , {_handleStrings(model.PhoneNumber)}, 
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
       

        /*
        public async void UpdateUser(UserModel model)
        {
            string sql = @$"UPDATE Users SET 
                Username = {_handleStrings(model.Username)}, 
                Image = {_handleStrings(model.Image)}, 
                Address = {_handleStrings(model.Address)}, 
                City = {_handleStrings(model.City)}, 
                Country = {_handleStrings(model.Country)}, 
                PhoneNumber = {_handleStrings(model.PhoneNumber)}, 
                Email = {_handleStrings(model.Email)}, 
                Notes = {_handleStrings(model.Notes)} 
                WHERE Id = {model.Id};";

            var cmd = await DatabaseCommandBuilder.BuildCommand(sql, null);
            int num = await cmd.ExecuteNonQueryAsync();

            cmd.Connection?.Close();
            cmd.Dispose();

            if (num <= 0)
            {
                throw new Exception("Can't update user");
            }
        }
        */



        private async void _UpdateModel(object model, string table)
        {
            List<string> identityProperties = ModelUtils.Get_Identity_Model_Property_Names(model);
            if (identityProperties.Count == 0)
            {
                throw new Exception("Model has no identity properties. For example: __Id");
            }

            StringBuilder stringBuilder = new StringBuilder("UPDATE ")
                .Append(table).Append(" SET ");

            List<string> properties = ModelUtils.Get_View_Model_Property_Names(model);

            _SetSQL_Values(ref stringBuilder, model, properties, ", ");

            stringBuilder.Append(" WHERE ");

            _SetSQL_Values(ref stringBuilder, model, identityProperties, " AND ");
            stringBuilder.Append(';');

            /*
                string sql = @$"UPDATE Users SET 
                Username = {_handleStrings(model.Username)}, 
                Image = {_handleStrings(model.Image)}, 
                Address = {_handleStrings(model.Address)}, 
                City = {_handleStrings(model.City)}, 
                Country = {_handleStrings(model.Country)}, 
                PhoneNumber = {_handleStrings(model.PhoneNumber)}, 
                Email = {_handleStrings(model.Email)}, 
                Notes = {_handleStrings(model.Notes)} 
                WHERE Id = {model.Id};";*/

            Console.WriteLine("Update Model SQL: " + stringBuilder.ToString());

            var cmd = await DatabaseCommandBuilder.BuildCommand(
                stringBuilder.ToString(), null);
            int num = await cmd.ExecuteNonQueryAsync();

            cmd.Connection?.Close();
            cmd.Dispose();

            if (num <= 0)
            {
                throw new Exception("Can't update model");
            }
        }

        private async void _InsertModel(object model, string table)
        {
            
            StringBuilder stringBuilder = new StringBuilder("INSERT INTO ")
                .Append(table).Append(" (");

            List<string> properties = ModelUtils.Get_View_Model_Property_Names(model);
            properties.AddRange(ModelUtils.Get_Exceptional_Model_Property_Names(model));
            // username, address, _email, _password

            _ListProperties(ref stringBuilder, properties);

            stringBuilder.Append(") VALUES (");

            _ListValues(ref stringBuilder, properties, model);

            stringBuilder.Append(");");


            Console.WriteLine("Insert Model SQL: " + stringBuilder.ToString());

            /*
             * string sql = @$"INSERT INTO Users 
    (Username, Password, Image, Address, City, Country, PhoneNumber, Email, Notes) VALUES 
    ({_handleStrings(model.Username)}, '{hashedPass}', {_handleStrings(model.Image)}, 
    {_handleStrings(model.FullAddress)}, {_handleStrings(model.City)}, {_handleStrings(model.Country)}
    , {_handleStrings(model.PhoneNumber)}, 
    {email}, {_handleStrings(model.Notes)}); 
        SELECT * FROM Users WHERE Email = {email} AND Password = '{hashedPass}'; */

            var cmd = await DatabaseCommandBuilder.BuildCommand(
                stringBuilder.ToString(), null);
            int num = await cmd.ExecuteNonQueryAsync();

            cmd.Connection?.Close();
            cmd.Dispose();

            if (num <= 0)
            {
                throw new Exception("Can't insert model");
            }
        }

        private async void _DeleteModel(object model, string table)
        {
            StringBuilder stringBuilder = new StringBuilder("DELETE FROM ")
                .Append(table).Append(" WHERE ");

            _SetSQL_Values(ref stringBuilder, model, 
                ModelUtils.Get_Identity_Model_Property_Names(model), " AND ");

            Console.WriteLine("Delete Model SQL: "+stringBuilder.ToString());

            var cmd = await DatabaseCommandBuilder.BuildCommand(
                stringBuilder.ToString(), null);
            int num = await cmd.ExecuteNonQueryAsync();

            cmd.Connection?.Close();
            cmd.Dispose();

            if (num <= 0)
            {
                throw new Exception("Can't delete model");
            }
        }

        private async Task<object?> _SelectModel(object model, string table, 
            List<string> whereProperties)
        {
            // todo add checks for "where" and limiting and ordering
            StringBuilder stringBuilder = new StringBuilder("SELECT * FROM ")
                .Append(table).Append(" WHERE ");

            _SetSQL_Values(ref stringBuilder, model,
                whereProperties, " AND ");

            Console.WriteLine("Select Model SQL: " + stringBuilder.ToString());

            var cmd = await DatabaseCommandBuilder.BuildCommand(
                stringBuilder.ToString(), null);
            
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            object? res = null;
            while (await reader.ReadAsync())
            {
                res = ConvertToModel(reader, model);
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();
            return res;

        }

        public void UpdateUser2(object model)
        {
            _UpdateModel(model, "Users");
        }

        public void InsertUser2(object model)
        {
            _InsertModel(model, "Users");
        }

        public async Task<UserModel?> GetUser2(object model, 
            List<string> whereProperties)
        {
            object? obj = await _SelectModel(model, "Users", whereProperties);
            return obj == null ? null : (UserModel) obj;
        }


        public static string _hashString(string str) {
            str += _hashingSlat;
            return Encoding.ASCII.GetString(
                Program.hashing?.ComputeHash(Encoding.ASCII.GetBytes(str)) ?? []);
        }

        private static string _handleStrings(object str) {
            return str == null || ((string)str).Replace(" ", "").Length == 0 ? 
                "null" : "'" + ((string)str).Replace("'", "''") + "'";
        }


        private static object _handlePropertyValue(object model, string property)
        {
            object value = ModelUtils.Get_Property_Value(model, property);
            if (value is String)
            {
                value = _handleStrings(value);
            }
            return value;
        }

        private static void _SetValue_With_Property(ref StringBuilder stringBuilder, 
            string property, object model)
        {
            stringBuilder.Append(property);
            stringBuilder.Append(" = ");
            stringBuilder.Append(_handlePropertyValue(model, property));
        }


        private static void _ListProperties(ref StringBuilder stringBuilder, 
            List<string> properties)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                string property = properties[i];
                stringBuilder.Append(property)
                    .Append(i == properties.Count - 1 ? " " : ", ");
            }
        }

        private static void _ListValues(ref StringBuilder stringBuilder,
            List<string> properties, object model)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                stringBuilder.Append(_handlePropertyValue(model, properties[i]))
                    .Append(i == properties.Count - 1 ? " " : ", ");
            }
        }

        private static void _SetSQL_Values(ref StringBuilder stringBuilder, object model, 
            List<string> properties, string ending_suffix)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                string property = properties[i];
                _SetValue_With_Property(ref stringBuilder, property, model);
                stringBuilder.Append(i == properties.Count - 1 ? " " : ending_suffix);
            }
        }

        private static object? ConvertToModel(NpgsqlDataReader reader, object model)
        {
            object obj;
            if (model is UserModel){

                obj = new UserModel();

            } else if (model is DishModel) {
                obj = new DishModel();

            } else if (model is CuponModel) {
                obj = new CuponModel();

            } else {
                return null;
            }

            foreach (string property in ModelUtils.Get_All_Model_Property_Names(model))
            {
                ModelUtils.Set_Property_Value(obj, property, reader[property.ToLower()]);
            }
            return obj;
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

            foreach (string property in ModelUtils.Get_All_Model_Property_Names(user))
            {
                ModelUtils.Set_Property_Value(user, property, reader[property.ToLower()]);
            }
            /*
            user.Username = Convert.ToString(reader["username"]);
            user.Image = Convert.ToString(reader["image"]);
            user.Address = Convert.ToString(reader["address"]);
            user.City = Convert.ToString(reader["city"]);
            user.Country = Convert.ToString(reader["country"]);
            user.PhoneNumber = Convert.ToString(reader["phonenumber"]);
            user.Email = Convert.ToString(reader["email"]);
            user.Notes = Convert.ToString(reader["notes"]);
            user.__Id = Convert.ToInt32(reader["__id"]);*/
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
