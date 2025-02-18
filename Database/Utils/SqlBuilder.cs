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
            return sql.Append(';').ToString();
        }

        public SqlBuilder Select(string fields, string table)
        {
            // If you have a list of fields then you can do `string.Join(", ", List)` to get the string.
            sql.Append("SELECT ").Append(fields).Append(" FROM ")
                .Append(table).Append(' ');
            return this;
        }

        public SqlBuilder Update(string table)
        {
            sql.Append("UPDATE ").Append(table).Append(' ');
            return this;
        }

        public SqlBuilder Insert(string table, object model)
        {
            sql.Append("INSERT INTO ").Append(table).Append(" (");

            List<string> properties = ModelUtils.Get_Model_Property_Names(model);
            sql.Append(string.Join(", ", properties))
                .Append(") VALUES (");

            List<object> values = new List<object>();
            foreach (string property in properties)
            {
                values.Add(ValueHandler.GetModelPropertyValue(model, property));
            }

            sql.Append(string.Join(", ", values))
                .Append(") ");
            return this;
        }

        public SqlBuilder Where_Set(string keyword, Dictionary<string, object> values)
        {
            // key = Field ; value = Field condition (the value it needs to be handled before passing it here)
            sql.Append(' ').Append(keyword).Append(' ');
            int i = 0;
            foreach (string key in values.Keys)
            {
                sql.Append(key).Append(" = ")
                    .Append(values[key])
                    .Append(i == values.Keys.Count - 1 ? " " : " AND ");
                i++;
            }
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
    }
}
