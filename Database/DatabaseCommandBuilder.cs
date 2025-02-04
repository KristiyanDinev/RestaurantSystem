using Npgsql;

namespace ITStepFinalProject.Database {
    public class DatabaseCommandBuilder {
        public static async Task<NpgsqlCommand> BuildCommand(string sql, 
            List<NpgsqlParameter>? args = null) {
            var command = new NpgsqlCommand(sql);
            if (args != null) {
                command.Parameters.AddRange(args.ToArray());
            }
            DatabaseConnection databaseConnection = new DatabaseConnection();
            command.Connection = await databaseConnection.GetConnectionAsync();
            return command;
        }

    }
}
