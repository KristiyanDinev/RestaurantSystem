using Npgsql;

namespace ITStepFinalProject.Models
{
    public class ResultSqlQuery
    {
        public List<object> Models { get; set; }
        public NpgsqlCommand? Cmd { get; set; }
    }
}
