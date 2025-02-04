using ITStepFinalProject.Models;
using Npgsql;
using NpgsqlTypes;
using System.Reflection;
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

            UserModel model = new UserModel();
            model.Id = id;

            var cmd = await DatabaseCommandBuilder.BuildCommand(
                sql, npgsqlParameters);
            cmd.Prepare();
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) {
                model.Username = Convert.ToString(reader["username"]) ?? "";
                model.Image = Convert.ToString(reader["image"]);
                model.Address = Convert.ToString(reader["address"]);
                model.PhoneNumber = Convert.ToString(reader["phonenumber"]);
                model.Email = Convert.ToString(reader["email"]);
                model.Notes = Convert.ToString(reader["Notes"]);
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();
            return model;
        }

        public async Task<int> RegisterUser(UserModel model, string password) {

            string sql = @$"INSERT INTO Users 
    (Username, Password, Image, Address, PhoneNumber, Email, Notes) VALUES 
    ({_handleStrings(model.Username)}, '{_hashString(password)}', {_handleStrings(model.Image)}, 
    {_handleStrings(model.Address)}, {_handleStrings(model.PhoneNumber)}, 
    {_handleStrings(model.Email)}, {_handleStrings(model.Notes)})";

            var cmd = await DatabaseCommandBuilder.BuildCommand(sql, null);
            
            
            int num = await cmd.ExecuteNonQueryAsync();
            cmd.Connection?.Close();
            cmd.Dispose();

            if (num == 1) {
                int Id = await LoginUser(model.Email, password);
                return Id;

            } else {
                throw new Exception("Didn't register");
            }
        }

        public async Task<int> LoginUser(string email, string password) {

            string sql = $"SELECT Id FROM Users WHERE Email = {_handleStrings(email)} AND Password = '{_hashString(password)}' LIMIT 1";

            var cmd = await DatabaseCommandBuilder.BuildCommand(sql, null);
            cmd.Prepare();
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) {
                object id = reader["id"];
                reader.Close();
                cmd.Connection?.Close();
                cmd.Dispose();
                return Convert.ToInt32(id);
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
                DishModel dish = new DishModel();
                dish.Id = Convert.ToInt32(reader["id"]);
                dish.Name = Convert.ToString(reader["name"]) ?? "";
                dish.Grams = Convert.ToInt32(reader["grams"]);
                dish.AvrageTimeToCook = Convert.ToString(reader["avragetimetocook"]) ?? "";
                dish.Type_Of_Dish = type;
                dish.Price = float.Parse(Convert.ToString(reader["price"]) ?? "00.00");
                dish.Image = Convert.ToString(reader["image"]);
                dish.IsAvailable = bool.Parse(Convert.ToString(reader["isavailable"]) ?? "true");
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();
            return dishModels;
        }

        public static string _hashString(string str) {
            return Encoding.ASCII.GetString(
                Program.hashing?.ComputeHash(Encoding.ASCII.GetBytes(str)) ?? []);
        }

        private static string _handleStrings(string? str) {
            return str == null || str.Replace(" ", "").Length == 0 ? 
                "null" : "'" + str.Replace("'", "''") + "'";
        }
    }
}
