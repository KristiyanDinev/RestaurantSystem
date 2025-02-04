using Npgsql;

namespace ITStepFinalProject.Database {
    public class DatabaseConnection : IDisposable {
        public static string _connectionString = "";
        private NpgsqlConnection? _connection;

        public async Task<NpgsqlConnection> GetConnectionAsync() {
            if (_connection == null || _connection.State != System.Data.ConnectionState.Open) {
                _connection = new NpgsqlConnection(_connectionString);
                await _connection.OpenAsync();
            }
            return _connection;
        }

        public async void CheckConnection() {
            if (await GetConnectionAsync() == null) {
                throw new Exception("Connection Invalid");
            }
        }

        public void Dispose() {
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
