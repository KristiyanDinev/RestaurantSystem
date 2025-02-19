using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using Npgsql;
using System.Reflection;

namespace ITStepFinalProject.Database.Handlers
{
    public class UserDatabaseHandler
    {
        public async Task<UserModel> GetUser(int id)
        {
            /*
            List<NpgsqlParameter> npgsqlParameters = new List<NpgsqlParameter>();

            NpgsqlParameter idArg = new NpgsqlParameter("@Id", NpgsqlDbType.Integer);
            idArg.Value = id;

            npgsqlParameters.Add(idArg);

            //string sql = "SELECT * FROM Users WHERE Id = @Id";*/

            List<string> values = new List<string>();
            values.Add("Id = "+ id);

            List<object> user = await 
                DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("*", "User")
                .Where_Set_On_Having("WHERE", values).ToString(), new UserModel(), true);

            return (UserModel)user[0];
        }

        public async void RegisterUser(UserModel model)
        {
            model.Password = ValueHandler.HashString(model.Password);

            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Insert("User", [model]).ToString());
        }

        public async Task<UserModel> LoginUser(UserModel loginUser)
        {
            List<string> values = new List<string>();
            values.Add("Email = "+ ValueHandler.Strings(loginUser.Email)+" AND ");
            values.Add("Password = '" + ValueHandler.HashString(loginUser.Password) + '\'');

            SqlBuilder sqlBuilder = new SqlBuilder()
                .Select("*", "Users")
                .Where_Set_On_Having("WHERE", values);

            string v = sqlBuilder.ToString();

            Console.WriteLine("Sql Builder: " + v);

            List<object> user = await DatabaseManager.
                _ExecuteQuery(v, loginUser, false);
            return (UserModel)user[0];
        }

        /*
         * <summery>
         * `model` must have one or more properties/fields
         * </summery>
         */
        public async void UpdateUser(UserModel model)
        {
            List<string> values = new List<string>();
            values.Add("Password = '" + ValueHandler.HashString(model.Password) + "' AND ");

            List<string> names = ModelUtils.Get_Model_Property_Names(model);
            for (int i = 0; i < names.Count; i++)
            {
                string property = names[i];
                if (property.Equals("Password") || property.Equals("Id"))
                {
                    continue;
                }
                values.Add(property + 
                    " = "+ ValueHandler.GetModelPropertyValue(model, property) +
                    (i == names.Count - 1 ? "" : " AND "));
            }

            List<string> where = new List<string>();
            where.Add("Id = "+model.Id);

            DatabaseManager._UpdateModel("User", values, where);
        }

        public async void DeleteUser(UserModel model)
        {
            List<string> values = new List<string>();
            values.Add("Id = " + model.Id);

            DatabaseManager._ExecuteNonQuery(
                new SqlBuilder().Delete("User")
                .Where_Set_On_Having("WHERE", values).ToString());
        }
    }
}
