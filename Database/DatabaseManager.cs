using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using Npgsql;

namespace ITStepFinalProject.Database {
    public class DatabaseManager {
        // Username, password and email are required
        // Username: 123
        // email: 123@example.com
        // password: 123

        /*
         * 
          insert into Dishes (Name, Price, Ingredients, Grams, Type_Of_Dish, IsAvailable, AvrageTimeToCook, Image)
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
('Dishes 2', 30.22, 'some stuff in it and this and that', 120, 'dishes', false, '3', null);
       
        -- year-month-day
          insert into Cupons (CuponCode, DiscountPercent, ExpirationDate, Name) VALUES
          ('Code', 5.99, '2022-01-01', 'Expired'),
          ('Summer', 6.10, '2025-05-01', 'Summer Cupon'),
          ('Winter', 12.99, '2025-12-01', 'Winter Cupon');


        -- MM/DD/YYYYTHH:mm:ssZ
         */

        public static string _connectionString = "";
        public static async Task<NpgsqlCommand> BuildCommand(string sql)
        {
            var command = new NpgsqlCommand(sql);
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            command.Connection = connection;
            return command;
        }



        public static async void Setup() {
            string sql = """
                CREATE TABLE IF NOT EXISTS Users (
                    Id SERIAL PRIMARY KEY,
                    Username VARCHAR(100) NOT NULL UNIQUE,
                    Password VARCHAR(64) NOT NULL,
                    Image VARCHAR(255),
                    Address TEXT NOT NULL,
                    City VARCHAR(100) NOT NULL,
                    State VARCHAR(100),
                    Country VARCHAR(100) NOT NULL,
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
                    RestorantAddress TEXT NOT NULL,
                    RestorantCity VARCHAR(100) NOT NULL,
                    RestorantState VARCHAR(100),
                    RestorantCountry VARCHAR(100) NOT NULL,
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
                    ExpirationDate DATE NOT NULL,
                    Name VARCHAR(100) NOT NULL
                );

                """;


            var cmd = await BuildCommand(sql);
            await cmd.ExecuteNonQueryAsync();
            cmd.Dispose();
        }


        public static object ConvertToModel(NpgsqlDataReader reader, object model)
        {
            object obj = Activator.CreateInstance(model.GetType());

           foreach (string property in ModelUtils.Get_Model_Property_Names(model))
           {
               try
               {
                  ModelUtils.Set_Property_Value(obj, property, reader[property.ToLower()]);
               }
               catch (Exception)
               { }
           }
            return obj;
        }

        public static async void _ExecuteNonQuery(string sql) {
            var cmd = await BuildCommand(sql);
            int num = await cmd.ExecuteNonQueryAsync();

            cmd.Connection?.Close();
            cmd.Dispose();

            if (num <= 0)
            {
                throw new Exception();
            }
        }

        public static async Task<List<object>> _ExecuteQuery(string sql,
            object model, bool isPrepare)
        {
            var cmd = await BuildCommand(sql);

            if (isPrepare)
            {
                cmd.Prepare();
            }

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<object> models = new List<object>();
            while (await reader.ReadAsync())
            {
                models.Add(ConvertToModel(reader, model));
            }
            reader.Close();
            cmd.Connection?.Close();
            cmd.Dispose();
            return models;
        }

        public static async void _UpdateModel(string table, List<string> set,
            List<string> where)
        {
            string sql = new SqlBuilder()
                .Update(table).Where_Set_On_Having("SET", set)
                .Where_Set_On_Having("WHERE", where).ToString();

            Console.WriteLine("Update SQL: " + sql);
            _ExecuteNonQuery(sql);
        }
    }
}
