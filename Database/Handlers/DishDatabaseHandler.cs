using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;

namespace ITStepFinalProject.Database.Handlers
{
    public class DishDatabaseHandler
    {
        private static readonly string table = "Dishes";
        public async Task<List<DishModel>> GetDishes(string type)
        {
            List<object> objects = await DatabaseManager.
                _ExecuteQuery(new SqlBuilder().Select("*", table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Type_Of_Dish", ValueHandler.Strings(type))
                .ToString(), new DishModel(), true);

            return objects.Cast<DishModel>().ToList();
        }

        public async Task<List<DishModel>> GetDishesByIds(List<int> IDs)
        {
            List<object> dishes = 
                await DatabaseManager._ExecuteQuery(new SqlBuilder().Select("*", table)
                .ConditionKeyword("WHERE")
                .BuildCondition("Id", "("+string.Join(", ", IDs)+")", "IN")
                .ToString(), new DishModel(), true);

            return dishes.Cast<DishModel>().ToList();
        }

    }
}
