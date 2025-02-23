using ITStepFinalProject.Utils;
using System.Text;

namespace ITStepFinalProject.Database.Utils
{
    public class SqlBuilder
    {
        private readonly StringBuilder sql;
        public SqlBuilder() {
            sql = new StringBuilder();
        }

        public override string ToString()
        {
            string v = sql.Append(';').ToString();
            Console.WriteLine("SQL Builder: " + v);
            return v;
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
                .Append(table).Append(' ');
            return this;
        }

        public SqlBuilder Update(string table)
        {
            sql.Append("UPDATE ").Append(table).Append(' ');
            return this;
        }

        public SqlBuilder Insert(string table, List<object> models)
        {
            if (models.Count == 0)
            {
                return this;
            }
            sql.Append("INSERT INTO ").Append(table).Append(" (");

            List<string> properties = ModelUtils.Get_Model_Property_Names(models[0]);
            sql.Append(string.Join(", ", properties))
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


        /*
         * <summery>
         * All the vules must be handled and set.
         * 
         * conditions -> a list of conditions. A condition is this:
         * "Username = 'Hi' AND " or "Email = 'something@example.com'"
         * </summery>
         */
        public SqlBuilder Where_Set_On_Having(string keyword, List<string> conditions)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string condition in conditions)
            {
                stringBuilder.Append(condition);
            }
            sql.Append(' ').Append(keyword).Append(' ')
                .Append(stringBuilder.ToString()).Append(' ');
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
            sql.Append("DELETE FROM ").Append(table).Append(' ');
            return this;
        }

        public SqlBuilder Group_By(string fields)
        {
            sql.Append(" GROUP BY ").Append(fields).Append(' ');
            return this;
        }

        public SqlBuilder Join(string table, string type_of_join)
        {
            sql.Append($" {type_of_join} JOIN ").Append(table).Append(' ');
            return this;
        }

        public SqlBuilder Order_By(string fields)
        {
            sql.Append(" ORDER BY ").Append(fields).Append(' ');
            return this;
        }
    }
}
