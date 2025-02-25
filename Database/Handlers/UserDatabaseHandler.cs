using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models;
using ITStepFinalProject.Utils;

namespace ITStepFinalProject.Database.Handlers
{
    public class UserDatabaseHandler
    {
        private static readonly string table = "Users";
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
                .Select("*", table)
                .Where_Set_On_Having("WHERE", values).ToString(), new UserModel(), true);

            return (UserModel)user[0];
        }

        public async void RegisterUser(InsertUserModel model)
        {
            model.Password = ValueHandler.HashString(model.Password);
            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Insert(table, [model]).ToString());
        }

        public async Task<UserModel?> LoginUser(UserModel loginUser, bool hashPassword)
        {
            List<string> values = new List<string>();
            values.Add("Email = "+ ValueHandler.Strings(loginUser.Email)+" AND ");
            values.Add("Password = '" + 
                (hashPassword ? ValueHandler.HashString(loginUser.Password) :
                    loginUser.Password) + '\'');


            List<object> user = await DatabaseManager
                ._ExecuteQuery(new SqlBuilder()
                .Select("*", table)
                .Where_Set_On_Having("WHERE", values).ToString(), loginUser, true);
            return user.Count == 0 ? null : (UserModel)user[0];
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

            DatabaseManager._UpdateModel(table, values, where);
        }

        public async void DeleteUser(UserModel model)
        {
            List<string> values = new List<string>();
            values.Add("Id = " + model.Id);

            DatabaseManager._ExecuteNonQuery(
                new SqlBuilder().Delete(table)
                .Where_Set_On_Having("WHERE", values).ToString());
        }
    }
}
