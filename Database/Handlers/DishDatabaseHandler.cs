using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models;
using Npgsql;
using System.Linq;

namespace ITStepFinalProject.Database.Handlers
{
    public class DishDatabaseHandler
    {
        public async Task<List<DishModel>> GetDishes(string type)
        {
            List<string> res = new List<string>();
            res.Add("CuponCode = " + ValueHandler.Strings(type));

            List<object> objects = await DatabaseManager.
                _ExecuteQuery(new SqlBuilder().Select("*", "Dishes")
                .Where_Set_On_Having("WHERE", res).ToString(), new DishModel(), true);

            return objects.Cast<DishModel>().ToList();
        }

        public async Task<DishModel> GetDishById(int id)
        {
            List<string> res = new List<string>();
            res.Add("Id = "+ id);

            List<object> dishes = 
                await DatabaseManager._ExecuteQuery(new SqlBuilder().Select("*", "Dishes")
                .Where_Set_On_Having("WHERE", res).ToString(), new DishModel(), true);

            return (DishModel)dishes[0];
        }

    }
}
