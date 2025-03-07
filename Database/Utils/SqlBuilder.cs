using ITStepFinalProject.Utils.Utils;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace ITStepFinalProject.Database.Utils
{
    public class SqlBuilder
    {
        private readonly StringBuilder sql;
        private bool _isPostgresql;

        public SqlBuilder(bool isPostgresql = true) {
            sql = new StringBuilder();
            _isPostgresql = isPostgresql;
        }

        public override string ToString()
        {
            string v = sql.Append(';').ToString();
            Console.WriteLine("\nSQL Builder: " + v + "\n");
            return v;
        }

        private string _handleTableNames(string table)
        {
            return table.Length == 0 ? table : (_isPostgresql ? '"' + table.Replace(".", "\".\"") + '"' : table);
        }

        private string _handlePropertyNames(List<string> properties)
        {
            // ["Id", "Name", "Email"]
            // "Id", "Name", "Email"
            return _isPostgresql ? '"'+string.Join("\", \"", properties)+'"' : 
                string.Join(", ", properties);
        }

        /*
         * <summery> 
         * fileds: 
         * MySQL = "Name, Email"
         * MySQL with aliases = "Users.Name As UserName, User.Email AS UserEmail"
         * SqlServer = "TOP 3 Name, Email"
         * 
         * If you have a list of fields then you can do `string.Join(", ", List)` to get the string.
         * </summery>
        */
        public SqlBuilder Select(string fields, string table)
        {
            sql.Append("SELECT ").Append(fields).Append(" FROM ")
                .Append(_handleTableNames(table)).Append(' ');
            return this;
        }

        public SqlBuilder Update(string table)
        {
            sql.Append("UPDATE ").Append(_handleTableNames(table)).Append(' ');
            return this;
        }

        public SqlBuilder Insert(string table, List<object> models)
        {
            if (models.Count == 0)
            {
                return this;
            }
            sql.Append("INSERT INTO ").Append(_handleTableNames(table)).Append(" (");

            List<string> properties = ObjectUtils.Get_Model_Property_Names(models[0]);
            sql.Append(_handlePropertyNames(properties))
                .Append(") VALUES ");


            for (int i = 0; i < models.Count; i++)
            {
                object model = models[i];
                sql.Append('(');
                List<object> values = new List<object>();
                foreach (string property in properties)
                {
                    values.Add(ValueHandler.GetModelPropertyValue(model, property));
                }

                sql.Append(string.Join(", ", values))
                    .Append(')');

                if (i < models.Count - 1)
                {
                    sql.Append(", ");
                }
            }

            
            return this;
        }


        public SqlBuilder Where()
        {
            sql.Append(" WHERE ");
            return this;
        }

        public SqlBuilder Having()
        {
            sql.Append(" HAVING ");
            return this;
        }

        public SqlBuilder On()
        {
            sql.Append(" ON ");
            return this;
        }

        public SqlBuilder Set()
        {
            sql.Append(" SET ");
            return this;
        }

        public SqlBuilder BuildCondition(string property, object value, string condition = "=",
            string endingCondtion = "", string groupStart = " ", string groupEnd = " ")
        {
            sql.Append(groupStart).Append(_handleTableNames(property)).Append(' ')
                .Append(condition).Append(' ').Append(value)
                .Append(groupEnd).Append(endingCondtion);
            return this;
        }

        public SqlBuilder Limit(int limit, int? offset)
        {
            sql.Append(" LIMIT ").Append(limit);
            if (offset != null)
            {
                sql.Append(" OFFSET ").Append(offset);
            }
            return this;
        }

        public SqlBuilder Delete(string table)
        {
            sql.Append("DELETE FROM ").Append(_handleTableNames(table)).Append(' ');
            return this;
        }

        /*
         * Note: The "case-sensitive" of the database fields/properties will no automatily apply here
         */
        public SqlBuilder Group_By(string fields)
        {
            sql.Append(" GROUP BY ").Append(fields).Append(' ');
            return this;
        }

        public SqlBuilder Join(string table, string type_of_join)
        {
            sql.Append($" {type_of_join} JOIN ").Append(_handleTableNames(table)).Append(' ');
            return this;
        }

        /*
         * Note: The "case-sensitive" of the database fields/properties will no automatily apply here
         */
        public SqlBuilder Order_By(string fields)
        {
            sql.Append(" ORDER BY ").Append(fields).Append(' ');
            return this;
        }
    }
}
