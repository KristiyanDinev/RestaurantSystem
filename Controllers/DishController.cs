using ITStepFinalProject.Database;
using ITStepFinalProject.Models;

namespace ITStepFinalProject.Controllers {
    public class DishController {

        public DishController(WebApplication app) {
            app.MapGet("/dishes/{type:string}", async (HttpContext context, 
                DatabaseManager db, string type) => {

                    Dictionary<string, List<DishModel>> data = 
                        new Dictionary<string, List<DishModel>>();


                    try {
                       
                        List<DishModel> dishes = await db.GetDishes(type);
                        data.Add("dishes", dishes);
                        return data;

                    } catch (Exception) {
                        return data;
                    }

            });
        }
    }
}
