using ITStepFinalProject.Database;
using ITStepFinalProject.Models;

namespace ITStepFinalProject.Controllers
{
    public class CartController
    {

        public CartController(WebApplication app)
        {
            app.MapGet("/cart", async (HttpContext context, 
                DatabaseManager db) =>
            {
               
                try
                {
                    int? id = Utils.Utils.IsLoggedIn(context.Session);
                    if (id == null)
                    {
                        return Results.Redirect("/login");
                    }

                    string FileData = await Utils.Utils.GetFileContent("/cart");

                    UserModel user = await db.GetUser((int)id);

                    Utils.Utils._handleEntryInFile(ref FileData, user, "User");
                    Utils.Utils.ApplyUserBarElement(ref FileData, user);

                    return Results.Content(FileData, "text/html");

                }
                catch (Exception)
                {
                    return Results.BadRequest();
                }
            });
        }
    }
}
