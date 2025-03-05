using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels;
using ITStepFinalProject.Utils.Utils;

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

            List<object> user = await 
                DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("*", table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Id", id).ToString(), new UserModel(), true);

            return (UserModel)user[0];
        }

        public void RegisterUser(InsertUserModel model)
        {
            model.Password = ValueHandler.HashString(model.Password);
            DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                .Insert(table, [model]).ToString(), true);
        }

        public async Task<UserModel?> LoginUser(UserModel loginUser, bool hashPassword)
        {
            List<object> user = await DatabaseManager
                ._ExecuteQuery(new SqlBuilder()
                .Select("*", table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Email", ValueHandler.Strings(loginUser.Email), "=", "AND")
                .BuildCondition("Password",
               "'"+ (hashPassword ? ValueHandler.HashString(loginUser.Password) :
                    loginUser.Password)+"'")
                .ToString(), loginUser, true);

            return user.Count == 0 ? null : (UserModel)user[0];
        }


        /*
         * <summery>
         * `model` must have one or more properties/fields
         * </summery>
         */
        public void UpdateUser(UserModel model)
        {
            SqlBuilder sqlBuilder = new SqlBuilder()
                .Update(table)
                .ConditionKeyword("SET")
                .BuildCondition("Password", ValueHandler.HashString(model.Password), ", ");

            List<string> names = ObjectUtils.Get_Model_Property_Names(model);
            for (int i = 0; i < names.Count; i++)
            {
                string property = names[i];
                if (property.Equals("Password") || property.Equals("Id"))
                {
                    continue;
                }
                sqlBuilder.BuildCondition(property, ValueHandler.GetModelPropertyValue(model, property),
                    i == names.Count - 1 ? "" : ", ");
            }

            sqlBuilder.ConditionKeyword("WHERE")
                .BuildCondition("Id", model.Id);

            DatabaseManager._ExecuteNonQuery(sqlBuilder.ToString(), true);
        }

        public void DeleteUser(int userId)
        {
            DatabaseManager._ExecuteNonQuery(
                new SqlBuilder().Delete(table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Id", userId)
                .ToString(), false);
        }
    }
}
