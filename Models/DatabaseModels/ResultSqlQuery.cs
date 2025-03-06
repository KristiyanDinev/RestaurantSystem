using Npgsql;

namespace ITStepFinalProject.Models.DatabaseModels
{
    public class ResultSqlQuery
    {
        public List<object> Models { get; set; }
        public NpgsqlCommand? Cmd { get; set; }
    }
}
