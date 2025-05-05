using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.View;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers
{
    public class AdminController
    {
        public AdminController(WebApplication app)
        {

            // Note: Here should be only endpoints that are for staff.
            // These endpoints are already Authorized by the Authentication middleware.

            /*
               app.MapControllerRoute(
                    name: "AdminTest",
                    pattern: "/admin2/{controller=Admin2}/{action=Index}");*/

            /*

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
        }*/
        }
    }


    public class Admin2Controller : Controller
    {
        private readonly string error = "Please update your profile or tell the Manager to update your profile address, city, state and country to align with the address of the restaurant you work in.";

        private RoleService _roleService;
        private OrderService _orderService;
        private UserUtility _userUtils;
        private RestaurantService _restaurantService;
        private OrderedDishesService _orderedDishesService;
        private ReservationService _reservationService;

        public Admin2Controller(UserUtility userUtils,
            Utility _utils, RoleService roleService,
            OrderService orderService, RestaurantService restaurantService,
            OrderedDishesService orderedDishesService, ReservationService reservationService)
        {
            _userUtils = userUtils;
            _roleService = roleService;
            _orderService = orderService;
            _restaurantService = restaurantService;
            _orderedDishesService = orderedDishesService;
            _reservationService = reservationService;
        }

        [HttpGet]
        [Route("Admin")]
        [Route("Admin/Index")]
        public async Task<IActionResult> Index()
        {
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/admin")))
            {
                return Redirect("/login");
            }

            return View(user);
        }


        [HttpGet]
        [Route("Admin/Dishes")]
        public async Task<IActionResult> Dishes()
        {
            // Chief in the kitchen

            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/admin/dishes")))
            {
                return Redirect("/login");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantForStaff(user);

            AdminDishesModel adminDishesModel = new AdminDishesModel();
            if (restaurant == null)
            {
                adminDishesModel.Error = error;
                return View(adminDishesModel);
            }

            Dictionary<OrderModel, List<DishModel>> dishes = new Dictionary<OrderModel, List<DishModel>>();
            foreach (OrderModel order in await _orderService.GetOrdersByRestaurantId(restaurant.Id))
            {
                dishes.Add(order, await _orderedDishesService.GetDishesFromOrder(order.Id));
            }

            adminDishesModel.restaurantModel = restaurant;
            adminDishesModel.userModel = user;
            adminDishesModel.dishes = dishes;

            return View(adminDishesModel);
        }


        [HttpGet]
        [Route("Admin/Reservations")]
        public async Task<IActionResult> Reservations()
        {
            // TODO Finish
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/admin/dishes")))
            {
                return Redirect("/login");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantForStaff(user);

            ReservationViewModel reservationViewModel = new ReservationViewModel();
            if (restaurant == null)
            {
                reservationViewModel.Error = error;
                return View(reservationViewModel);
            }

            _reservationService.
        }
    }
}
