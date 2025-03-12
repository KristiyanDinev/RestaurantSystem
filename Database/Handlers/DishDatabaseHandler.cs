using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models;
using ITStepFinalProject.Models.DatabaseModels;

namespace ITStepFinalProject.Database.Handlers
{
    public class DishDatabaseHandler
    {
        private static readonly string table = "Dishes";
        private static readonly string tableOrderedDishes = "OrderedDishes";
        public async Task<List<DishModel>> GetDishes(string type, int restorantId)
        {
            ResultSqlQuery objects = await DatabaseManager.
                _ExecuteQuery(new SqlBuilder().Select("*", table)
                .Where()
                .BuildCondition("Type_Of_Dish", ValueHandler.Strings(type), "=", "AND ")
                .BuildCondition("RestorantId", restorantId)
                .ToString(), new DishModel());

            return objects.Models.Cast<DishModel>().ToList();
        }

        public async Task<List<DishModel>> GetDishesByIds(List<int> IDs)
        {
            ResultSqlQuery dishes =
                await DatabaseManager._ExecuteQuery(new SqlBuilder().Select("*", table)
                .Where()
                .BuildCondition("Id", "(" + string.Join(", ", IDs) + ")", "IN")
                .ToString(), new DishModel());

            return dishes.Models.Cast<DishModel>().ToList();
        }

        public async Task UpdateDishStatusById(int id, string status)
        {
            await DatabaseManager._ExecuteNonQuery(
                new SqlBuilder()
                .Update(tableOrderedDishes)
                .Set()
                .BuildCondition("CurrentStatus", ValueHandler.Strings(status))
                .Where()
                .BuildCondition("DishId", id).ToString());
        }

        public async Task<string?> GetSameCurrentStatusForAllDishesByOrderId(int orderId)
        {
            ResultSqlQuery res = await DatabaseManager._ExecuteQuery(new SqlBuilder()
                .Select("CurrentStatus", tableOrderedDishes)
                .Where()
                .BuildCondition("OrderId", orderId).ToString(), new OrderedDishesModel());
            string? status = null;
            foreach (OrderedDishesModel model in res.Models.Cast<OrderedDishesModel>())
            {
                if (status == null)
                {
                    status = model.CurrentStatus;
                    continue;
                }
                if (!status.Equals(model.CurrentStatus))
                {
                    status = null;
                    break;
                }
            }
            return status;
        }
    }
}
