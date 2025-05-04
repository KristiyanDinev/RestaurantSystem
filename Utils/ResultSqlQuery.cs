using Npgsql;

namespace RestaurantSystem.Utils
{
    public class ResultSqlQuery
    {
        public List<object> Models { get; set; }
        public NpgsqlCommand? Cmd { get; set; }
    }
}
