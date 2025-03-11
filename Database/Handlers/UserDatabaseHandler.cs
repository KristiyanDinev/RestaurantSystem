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

            ResultSqlQuery user = await 
                DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("*", table)
                .Where()
                .BuildCondition("Id", id).ToString(), new UserModel());

            return (UserModel)user.Models[0];
        }

        public async Task<object?> RegisterUser(UserModel model)
        {
            model.Password = ValueHandler.HashString(model.Password);
            await DatabaseManager._ExecuteNonQuery(new SqlBuilder()
                    .Insert(table, [model], ["Id", "CreatedAt"]).ToString());
            return null;
        }

        public async Task<UserModel?> LoginUser(UserModel loginUser, bool hashPassword)
        {
            ResultSqlQuery user = await DatabaseManager
                ._ExecuteQuery(new SqlBuilder()
                .Select("*", table)
                .Where()
                .BuildCondition("Email", ValueHandler.Strings(loginUser.Email), "=", "AND")
                .BuildCondition("Password",
               "'"+ (hashPassword ? ValueHandler.HashString(loginUser.Password) :
                    loginUser.Password)+"'")
                .ToString(), loginUser);

            return user.Models.Count == 0 ? null : (UserModel)user.Models[0];
        }


        /*
         * <summery>
         * `model` must have one or more properties/fields
         * `model` must have a hashed password
         * </summery>
         */
        public async Task UpdateUser(UserModel model)
        {
            //model.Password = ValueHandler.HashString(model.Password);
            SqlBuilder sqlBuilder = new SqlBuilder()
                .Update(table)
                .Set();

            List<string> names = ObjectUtils.Get_Model_Property_Names(model);
            List<object> propertyValues = new List<object>();
            for (int i = 0; i < names.Count; i++)
            {
                string property = names[i];
                if (property.Equals("Id") || property.Equals("CreatedAt") 
                    || property.Equals("Password") || property.Equals("Email"))
                {
                    continue;
                }
                sqlBuilder.BuildCondition(property, ValueHandler.GetModelPropertyValue(model, property), "=",
                    i == names.Count - 1 ? "" : ", ");
            }

            sqlBuilder.Where().BuildCondition("Id", model.Id);

            await DatabaseManager._ExecuteNonQuery(sqlBuilder.ToString());
        }

        public async Task DeleteUser(int userId)
        {
            await DatabaseManager._ExecuteNonQuery(
                new SqlBuilder().Delete(table)
                .Where()
                .BuildCondition("Id", userId)
                .ToString());
        }
    }
}
