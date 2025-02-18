using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;
using Npgsql;
using System.Reflection;

namespace ITStepFinalProject.Database.Models
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

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Id", id);

            List<object> user = await 
                DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("*", "User")
                .Where_Set("WHERE", values).ToString(), new UserModel(), true);

            return (UserModel)user[0];
        }

        public async Task<UserModel> RegisterUser(UserModel model)
        {
            model.Password = ValueHandler.HashString(model.Password);

            List<object> user = await DatabaseManager.
                _ExecuteQuery(new SqlBuilder().Insert("User", model).ToString(),
                model, false);
            
            return (UserModel)user[0];
        }

        public async Task<UserModel> LoginUser(UserModel loginUser)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Email", ValueHandler.Strings(loginUser.Email));
            values.Add("Password", '\'' + ValueHandler.HashString(loginUser.Password) + '\'');

            SqlBuilder sqlBuilder = new SqlBuilder()
                .Select("*", "Users")
                .Where_Set("WHERE", values);

            string v = sqlBuilder.ToString();

            Console.WriteLine("Sql Builder: " + v);

            List<object> user = await DatabaseManager.
                _ExecuteQuery(v, loginUser, false);
            return (UserModel)user[0];
        }

        public async void UpdateUser(UserModel model)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Password", '\'' + ValueHandler.HashString(model.Password) + '\'');
            foreach (string property in ModelUtils.Get_Model_Property_Names(model))
            {
                if (property.Equals("Password"))
                {
                    continue;
                }
                values.Add(property, ValueHandler.GetModelPropertyValue(model, property));
            }

            DatabaseManager._UpdateModel("User", values);
        }

        public async void DeleteUser(UserModel model)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Id", model.Id);

            DatabaseManager._ExecuteNonQuery(
                new SqlBuilder().Delete("User")
                .Where_Set("WHERE", values).ToString());
        }
    }
}
