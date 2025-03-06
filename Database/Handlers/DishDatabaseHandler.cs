using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;

namespace ITStepFinalProject.Database.Handlers
{
    public class DishDatabaseHandler
    {
        private static readonly string table = "Dishes";
        public async Task<List<DishModel>> GetDishes(string type, int restorantId)
        {
            ResultSqlQuery objects = await DatabaseManager.
                _ExecuteQuery(new SqlBuilder().Select("*", table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Type_Of_Dish", ValueHandler.Strings(type), "=", "AND ")
                .BuildCondition("RestorantId", restorantId)
                .ToString(), new DishModel());

            return objects.Models.Cast<DishModel>().ToList();
        }

        public async Task<List<DishModel>> GetDishesByIds(List<int> IDs)
        {
            ResultSqlQuery dishes = 
                await DatabaseManager._ExecuteQuery(new SqlBuilder().Select("*", table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Id", "("+string.Join(", ", IDs)+")", "IN")
                .ToString(), new DishModel());

            return dishes.Models.Cast<DishModel>().ToList();
        }

    }
}
