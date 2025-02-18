using Npgsql;
using System.Data.Common;

namespace ITStepFinalProject.Database {
    public class DatabaseCommandBuilder {
        public static string _connectionString = "";
        public static async Task<NpgsqlCommand> BuildCommand(string sql) {
            var command = new NpgsqlCommand(sql);
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            command.Connection = connection;
            return command;
        }

    }
}
