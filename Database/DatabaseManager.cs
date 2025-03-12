using ITStepFinalProject.Models;
using ITStepFinalProject.Utils.Utils;
using Npgsql;
using Npgsql.Schema;

namespace ITStepFinalProject.Database {
    public class DatabaseManager {
        // Username, password and email are required
        // Username: 123
        // email: 123@example.com
        // password: 123

        /*
         * 
       
        -- year-month-day MM/DD/YYYYTHH:mm:ssZ
          insert into "Cupons" ("CuponCode", "DiscountPercent", "ExpirationDate", "Name") VALUES
          ('Code', 5.99, '2022-01-01', 'Expired'),
          ('Summer', 6.10, '2025-05-01', 'Summer Cupon'),
          ('Winter', 12.99, '2025-12-01', 'Winter Cupon');


        insert into "Restorant" ("RestorantAddress", "RestorantCity", "RestorantState", 
            "RestorantCountry", "DoDelivery", "ServeCustomersInPlace") values 
            ('ul. Restorant', 'Sofia', null, 'Bulgaria', true, false),
            ('ul. Restorant2', 'Sofia', null, 'Bulgaria', true, false),
            ('ul. Restorant2 alt', 'Sofia', null, 'Bulgaria', true, true),
            ('ul. Restorant3 alt', 'Sofia', null, 'Bulgaria', true, true),
            ('ul. Restorant3', 'Sofia', null, 'Bulgaria', false, true),
            ('ul. Restorant4', 'Sofia', null, 'Bulgaria', false, true),
            ('ul. Restorant5', 'Sofia', 'Some State', 'Bulgaria', false, true),
            ('ul. Restorant7', 'Sofia', 'Some State', 'Bulgaria', true, false),
            ('ul. Restorant6', 'Sofia', 'Some State', 'Bulgaria', true, false);

        insert into "Dishes" ("Name", "Price", "Ingredients", "Grams", "Type_Of_Dish", 
        "IsAvailable", "AvrageTimeToCook", "Image", "RestorantId")
values
('Salad 1', 10.99, 'some stuff in it and this and that', 440, 'salad', true, '3', '/images/salad/1.png', 4),
('Salad 2', 12.99, 'some stuff in it and this and that', 500, 'salad', false, '2', null, 1),
('Drink 1', 13.99, 'some stuff in it and this and that', 100, 'drink', true, '1', null, 2),
('Drink 2', 10.00, 'some stuff in it and this and that', 100, 'drink', false, '1', null, 3),
('Appetizer 1', 10.09, 'some stuff in it and this and that', 440, 'appetizers', true, '3', null, 4),
('Appetizer 2', 20.19, 'some stuff in it and this and that', 140, 'appetizers', false, '4', null, 1),
('Desserts 1', 23.19, 'some stuff in it and this and that', 140, 'desserts', true, '1', null, 2),
('Desserts 2', 30.20, 'some stuff in it and this and that', 120, 'desserts', false, '4', null, 3),
('Dishes 1', 21.19, 'some stuff in it and this and that', 142, 'dishes', true, '2', null, 5),
('Dishes 2', 30.22, 'some stuff in it and this and that', 120, 'dishes', false, '3', null, 5);

        insert into "TimeTable" ("RestorantId", "UserAddress", "UserCity", "UserState",
            "UserCountry", "AvrageDeliverTime") values 
            (1, 'ul. User', 'Sofia', null, 'Bulgaria', '5m - 10m'),
            (2, 'ul. User', 'Sofia', null, 'Bulgaria', '7m - 8m'),
            (2, 'ul. User2', 'Sofia', null, 'Bulgaria', '2m - 3m+'),
            (3, 'ul. User', 'Sofia', null, 'Bulgaria', '2m - 3m'),
            (4, 'ul. User', 'Sofia', null, 'Bulgaria', '1m - 2m'),
            (4, 'ul. User3', 'Sofia', null, 'Bulgaria', '1m - 2m+'),
            (5, 'ul. User', 'Sofia', 'Some State', 'Bulgaria', '3m - 5m'),
            (6, 'ul. User', 'Sofia', 'Some State', 'Bulgaria', '2m - 4m');

        insert into "Services" ("Role", "Service") values
            ('staff', ''),
            ('owner', ''),
            ('waitress', ''),
            ('cook', ''),
            ('delivery', ''),

            ('waitress', '/orders'),
            ('owner', '/orders'),

            ('cook', '/dishes'),
            ('owner', '/dishes'),

            ('delivery', '/delivery'),
            ('owner', '/delivery'),

            ('owner', '/owner');



        insert into "Roles" ("UserId", "Role") values
            (1, 'staff');
         */

        public static string _connectionString = "";



        private static async Task<NpgsqlCommand> BuildCommand(string sql, 
            NpgsqlConnection? connection = null, bool beginTransaction = false)
        {
            if (connection == null)
            {
                connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
            }

            NpgsqlTransaction? transaction = null;
            if (beginTransaction)
            {
                transaction = await connection.BeginTransactionAsync();
            }

            var command = new NpgsqlCommand(sql, connection, transaction);
            return command;
        }



        public static async void Setup() {
            string sql = """
                CREATE TABLE IF NOT EXISTS "Users" (
                    "Id" SERIAL PRIMARY KEY,
                    "Username" VARCHAR(100) NOT NULL,
                    "Password" VARCHAR(64) NOT NULL,
                    "Image" VARCHAR(255),
                    "Address" TEXT NOT NULL,
                    "City" VARCHAR(100) NOT NULL,
                    "State" VARCHAR(100),
                    "Country" VARCHAR(100) NOT NULL,
                    "PhoneNumber" VARCHAR(15),
                    "Email" VARCHAR(50) NOT NULL UNIQUE,
                    "Notes" VARCHAR(255),
                    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
                );

                CREATE TABLE IF NOT EXISTS "Restorant" (
                    "Id" SERIAL PRIMARY KEY,
                    "RestorantAddress" TEXT NOT NULL,
                    "RestorantCity" VARCHAR(100) NOT NULL,
                    "RestorantState" VARCHAR(100),
                    "RestorantCountry" VARCHAR(100) NOT NULL,
                    "DoDelivery" BOOL NOT NULL,
                    "ServeCustomersInPlace" BOOL NOT NULL,
                    "ReservationMaxChildren" SMALLINT NOT NULL DEFAULT -1,
                    "ReservationMinChildren" SMALLINT NOT NULL DEFAULT 0,
                    "ReservationMaxAdults" SMALLINT NOT NULL DEFAULT -1,
                    "ReservationMinAdults" SMALLINT NOT NULL DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS "Services" (
                    "Role" VARCHAR(100) NOT NULL,
                    "Service" VARCHAR(100) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS "Roles" (
                    "UserId" INT REFERENCES "Users"("Id") NOT NULL,
                    "Role" VARCHAR(100) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS "TimeTable" (
                    "RestorantId" INT REFERENCES "Restorant"("Id") NOT NULL,
                    "UserAddress" TEXT NOT NULL,
                    "UserCity" VARCHAR(100) NOT NULL,
                    "UserState" VARCHAR(100),
                    "UserCountry" VARCHAR(100) NOT NULL,
                    "AvrageDeliverTime" VARCHAR(50) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS "Dishes" (
                    "Id" SERIAL PRIMARY KEY,
                    "Name" VARCHAR(100) NOT NULL,
                    "Price" NUMERIC(10, 2) NOT NULL,
                    "Ingredients" TEXT NOT NULL,
                    "Grams" SMALLINT NOT NULL,
                    "Type_Of_Dish" VARCHAR(50) NOT NULL,
                    "IsAvailable" BOOLEAN NOT NULL DEFAULT TRUE,
                    "RestorantId" INT REFERENCES "Restorant"("Id") NOT NULL,
                    "AvrageTimeToCook" VARCHAR(20) NOT NULL,
                    "Image" TEXT
                );


                CREATE TABLE IF NOT EXISTS "Orders" (
                    "Id" SERIAL PRIMARY KEY,
                    "CurrentStatus" VARCHAR(100) NOT NULL,
                    "Notes" VARCHAR(255),
                    "OrderedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                    "UserId" INT REFERENCES "Users"("Id") NOT NULL,
                    "RestorantId" INT REFERENCES "Restorant"("Id") NOT NULL,
                    "TotalPrice" NUMERIC(10, 2) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS "OrderedDishes" (
                    "OrderId" INT REFERENCES "Orders"("Id") NOT NULL,
                    "DishId" INT REFERENCES "Dishes"("Id") NOT NULL,
                    "Notes" VARCHAR(255) DEFAULT NULL,
                    "CurrentStatus" VARCHAR(100) NOT NULL DEFAULT 'pending'
                );

                CREATE TABLE IF NOT EXISTS "Reservations" (
                    "Id" SERIAL PRIMARY KEY,
                    "ReservatorId" INT REFERENCES "Users"("Id"),
                    "Notes" VARCHAR(255),
                    "CurrentStatus" VARCHAR(100) NOT NULL,
                    "RestorantId" INT REFERENCES "Restorant"("Id") NOT NULL,
                    "Amount_Of_Adults" SMALLINT NOT NULL DEFAULT 0,
                    "Amount_Of_Children" SMALLINT NOT NULL DEFAULT 0,
                    "At_Date" TIMESTAMP WITH TIME ZONE NOT NULL,
                    "Created_At" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                    "Price_Per_Adult" NUMERIC(10, 2) NOT NULL DEFAULT 0,
                    "Price_Per_Children" NUMERIC(10, 2) NOT NULL DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS "Cupons" (
                    "CuponCode" VARCHAR(25) NOT NULL UNIQUE,
                    "DiscountPercent" NUMERIC(10, 2) NOT NULL,
                    "ExpirationDate" DATE NOT NULL,
                    "Name" VARCHAR(100) NOT NULL
                );

                """;


            var cmd = await BuildCommand(sql);
            await cmd.ExecuteNonQueryAsync();
            cmd.Dispose();
        }


        // the model shall not be null
        public static async Task<List<object>> ConvertToModels(NpgsqlDataReader reader, object model)
        {
            List<object> objects = new List<object>();
            if (!reader.HasRows)
            {
                return objects;
            }

            //DataTable ? dataTable = reader.GetSchemaTable();

            // create mapping for the columns
            Dictionary<string, string> columnMapping = new Dictionary<string, string>();
            foreach (NpgsqlDbColumn col in reader.GetColumnSchema())
            {
                string dataColumnName = col.ColumnName;
                if (columnMapping.ContainsKey(dataColumnName))
                {
                    continue;
                }

                if (dataColumnName.Contains('.'))
                {
                    columnMapping.Add(dataColumnName, dataColumnName.Split('.')[1]);

                } else
                {
                    columnMapping.Add(dataColumnName, dataColumnName);
                }
            }


            // while (await reader.ReadAsync())
            while (await reader.ReadAsync())
            {
                object obj = Activator.CreateInstance(model.GetType());
                // every record
                foreach (string key in columnMapping.Keys)
                {
                    try
                    {
                        ObjectUtils.Set_Property_Value(obj,
                            columnMapping[key], reader[key]);
                    } catch (Exception) { }
                }
                objects.Add(obj);
            }
            /*
            foreach (string property in ObjectUtils.Get_Model_Property_Names(model))
           {
               try
               {
                  ObjectUtils.Set_Property_Value(obj, property, reader[property.ToLower()]);
               }
               catch (Exception)
               { }
           }*/
            return objects;
        }

        public static async Task<NpgsqlCommand?> _ExecuteNonQuery(string sql,
            bool beginTransaction = false, NpgsqlConnection? connection = null) {

            var cmd = await BuildCommand(sql, connection, beginTransaction);
            cmd.Prepare();

            int num = await cmd.ExecuteNonQueryAsync();

            if (connection != null || beginTransaction)
            {
                return cmd;
            }

            cmd.Connection?.Close();
            cmd.Dispose();
            return null;
        }

        public static async Task<ResultSqlQuery> _ExecuteQuery(string sql,
            object model, bool beginTransaction = false, NpgsqlConnection? connection = null)
        {

            ResultSqlQuery res = new ResultSqlQuery();

            var cmd = await BuildCommand(sql, connection, beginTransaction);
            cmd.Prepare();

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<object> models = await ConvertToModels(reader, model);
            reader.Close();

            res.Models = models;
            res.Cmd = cmd;

            if (connection == null)
            {
                cmd.Connection?.Close();
                cmd.Dispose();
            }
            return res;
        }
    }
}
