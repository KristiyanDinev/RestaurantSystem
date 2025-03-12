using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Models.WebModels;
using ITStepFinalProject.Utils.Controller;
using ITStepFinalProject.Utils.Web;

namespace ITStepFinalProject.Controllers
{
    public class AdminController
    {
        public AdminController(WebApplication app)
        {

            // Note: Here should be only endpoints that are for staff.
            // These endpoints are already Authorized by the Authentication middleware.
            
            app.MapGet("/admin", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils, 
                ServiceDatabaseHandler serviceDatabaseHandler) => {

                    try
                    {
                        UserModel? user = await userUtils.GetUserModelFromAuth(context);
                        if (user == null)
                        {
                            return Results.Redirect("/login");
                        }

                        string FileData = await controllerUtils.GetHTMLFromWWWROOT("/admin");

                        RestorantModel restorantModel = await serviceDatabaseHandler.GetStaffRestorant(user);

                        FileData = webUtils.HandleCommonPlaceholders(FileData, controllerUtils.UserModelName, [user]);

                        FileData = webUtils.HandleCommonPlaceholders(FileData, 
                            controllerUtils.RestorantModelName, [restorantModel]);

                        return Results.Content(FileData, "text/html");

                    }
                    catch (Exception)
                    {
                        return Results.Redirect("/error");
                    }
                });


            app.MapGet("/admin/dishes", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils,
                ServiceDatabaseHandler serviceDatabaseHandler, 
                OrderDatabaseHandler orderDatabaseHandler) => {

                    try
                    {
                        UserModel? user = await userUtils.GetUserModelFromAuth(context);
                        if (user == null)
                        {
                            return Results.Redirect("/login");
                        }

                        string FileData = await controllerUtils.GetHTMLFromWWWROOT("/admin/cook");
                        RestorantModel restorantModel = await serviceDatabaseHandler.GetStaffRestorant(user);

                        FileData = webUtils.HandleCommonPlaceholders(FileData, controllerUtils.UserModelName, [user]);

                        FileData = webUtils.HandleCommonPlaceholders(FileData,
                            controllerUtils.RestorantModelName, [restorantModel]);

                        List<OrderModel> orderModels = await orderDatabaseHandler.GetOrdersByRestorantId(restorantModel.Id);
                        foreach (OrderModel orderModel in orderModels)
                        {

                            List<DishModel> dishes = await orderDatabaseHandler.GetAllDishesFromOrder(orderModel.Id);

                            List<DisplayDishModel> displayDishModels = controllerUtils.ConvertToDisplayDish(dishes);

                            foreach (DisplayDishModel dish in displayDishModels)
                            {
                                dish.OrderId = orderModel.Id;
                            }

                            FileData = webUtils.HandleCommonPlaceholders(FileData,
                                controllerUtils.OrderModelName, [orderModel]);

                            controllerUtils.ConvertToDisplayDish(dishes);

                            FileData = webUtils.HandleCommonPlaceholders(FileData,
                                controllerUtils.DishModelName,
                                displayDishModels
                                .Cast<object>().ToList());
                        }

                        FileData = webUtils.HandleCommonPlaceholders(FileData,
                               controllerUtils.OrderModelName, []);
                        


                        return Results.Content(FileData, "text/html");

                    }
                    catch (Exception)
                    {
                        return Results.Redirect("/error");
                    }
                });

            app.MapGet("/admin/orders", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils) => {

                    return await controllerUtils.HandleDefaultPage_WithUserModel("/admin/waitress",
                          context, userUtils, webUtils);
                });

            app.MapGet("/admin/delivery", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils) => {

                    return await controllerUtils.HandleDefaultPage_WithUserModel("/admin/delivery",
                          context, userUtils, webUtils);
                });

            app.MapGet("/admin/owner", async (HttpContext context,
                ControllerUtils controllerUtils, UserUtils userUtils, WebUtils webUtils) => {

                    return await controllerUtils.HandleDefaultPage_WithUserModel("/admin/owner",
                          context, userUtils, webUtils);
                });
        }
    }
}
