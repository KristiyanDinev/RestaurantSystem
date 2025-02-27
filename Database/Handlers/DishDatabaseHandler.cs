using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models.DatabaseModels;
using System.Linq;

namespace ITStepFinalProject.Database.Handlers
{
    public class DishDatabaseHandler
    {
        private static readonly string table = "Dishes";
        public async Task<List<DishModel>> GetDishes(string type)
        {
            List<string> res = new List<string>();
            res.Add("Type_Of_Dish = " + ValueHandler.Strings(type));

            List<object> objects = await DatabaseManager.
                _ExecuteQuery(new SqlBuilder().Select("*", table)
                .Where_Set_On_Having("WHERE", res).ToString(), new DishModel(), true);

            return objects.Cast<DishModel>().ToList();
        }

        public async Task<List<DishModel>> GetDishesByIds(List<int> IDs)
        {
            List<string> res = new List<string>();
            res.Add("Id in ("+string.Join(", ", IDs)+")");

            List<object> dishes = 
                await DatabaseManager._ExecuteQuery(new SqlBuilder().Select("*", table)
                .Where_Set_On_Having("WHERE", res).ToString(), new DishModel(), true);

            return dishes.Cast<DishModel>().ToList();
        }

    }
}
