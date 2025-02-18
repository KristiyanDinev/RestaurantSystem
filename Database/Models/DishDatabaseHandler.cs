using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models;
using Npgsql;
using System.Linq;

namespace ITStepFinalProject.Database.Models
{
    public class DishDatabaseHandler
    {
        public async Task<List<DishModel>> GetDishes(string type)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("Type_Of_Dish", ValueHandler.Strings(type));

            List<object> objects = await DatabaseManager.
                _ExecuteQuery(new SqlBuilder().Select("*", "Dishes")
                .Where_Set("WHERE", res).ToString(), new DishModel(), true);

            return objects.Cast<DishModel>().ToList();
        }

        public async Task<DishModel> GetDishById(int id)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("Id", id);

            List<object> dishes = 
                await DatabaseManager._ExecuteQuery(new SqlBuilder().Select("*", "Dishes")
                .Where_Set("WHERE", res).ToString(), new DishModel(), true);

            return (DishModel)dishes[0];
        }

    }
}
