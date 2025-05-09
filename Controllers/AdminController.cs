using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Models.View.Admin;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers
{

    [ApiController]
    [EnableRateLimiting("fixed")]
    public class AdminController : Controller
    {
        private readonly string _restaurant_error = "Please update your profile or tell the Manager to update your profile address, city, state and country to align with the address of the Restaurant you work in.";
        private readonly string _manager_restaurant_error = "Please update your profile or tell the Owner to update your profile address, city, state and country to align with the address of the Restaurant you work in.";

        private RoleService _roleService;
        private OrderService _orderService;
        private UserUtility _userUtils;
        private RestaurantService _restaurantService;
        private OrderedDishesService _orderedDishesService;
        private ReservationService _reservationService;
        private UserService _userService;
        private WebSocketService _webSocketService;

        public AdminController(UserUtility userUtils,
            Utility _utils, RoleService roleService,
            OrderService orderService, RestaurantService restaurantService,
            OrderedDishesService orderedDishesService, ReservationService reservationService,
            UserService userService, WebSocketService webSocketService)
        {
            _userUtils = userUtils;
            _roleService = roleService;
            _orderService = orderService;
            _restaurantService = restaurantService;
            _orderedDishesService = orderedDishesService;
            _reservationService = reservationService;
            _userService = userService;
            _webSocketService = webSocketService;
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
                return RedirectToAction("Login", "User");
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
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _userService.GetRestaurantWhereUserWorksIn(user);

            DishesViewModel adminDishesModel = new DishesViewModel();
            if (restaurant == null)
            {
                adminDishesModel.Error = _restaurant_error;
                return View(adminDishesModel);
            }

            Dictionary<OrderModel, List<DishModel>> dishes = new ();
            foreach (OrderModel order in await _orderService.GetOrdersByRestaurantId(restaurant.Id))
            {
                dishes.Add(order, await _orderedDishesService.GetDishesFromOrder(order.Id));
            }

            adminDishesModel.Restaurant = restaurant;
            adminDishesModel.Staff = user;
            adminDishesModel.Dishes = dishes;

            return View(adminDishesModel);
        }

        [HttpPost]
        [Route("Admin/Dishes")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DishesUpdate([FromForm] OrderUpdateFormModel orderUpdateFormModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Dishes");
            }

            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/admin/dishes")))
            {
                return RedirectToAction("Login", "User");
            }

            DishesViewModel adminDishesModel = new DishesViewModel();
            if (!await _orderService.UpdateOrderCurrentStatusById(orderUpdateFormModel.OrderId, 
                orderUpdateFormModel.OrderCurrentStatus))
            {
                adminDishesModel.Error = "Can't update order's current status.";
                return View("Dishes", adminDishesModel);
            }

            adminDishesModel.Staff = user;
            // TODO Add the rest of the model and return it.

            if (orderUpdateFormModel.DishCurrentStatus == null ||
                orderUpdateFormModel.DishId == null)
            {
                await _webSocketService.SendJsonToClient("/ws/orders", ..., 
                     _orderService.CheckIfOrderIsBeingTracked(user.Id, orderUpdateFormModel.OrderId));
                
                return View("Dishes");
            }

            
            return RedirectToAction("Dishes", adminDishesModel);
        }


        [HttpGet]
        [Route("Admin/Reservations")]
        public async Task<IActionResult> Reservations()
        {
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/admin/reservations")))
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _userService.GetRestaurantWhereUserWorksIn(user);
            AdminReservationViewModel reservationViewModel = new AdminReservationViewModel();

            if (restaurant == null)
            {
                reservationViewModel.Error = _restaurant_error;
                return View(reservationViewModel);
            }

            reservationViewModel.Staff = user;
            reservationViewModel.Reservations = await _reservationService
                .GetReservationsByRestaurantId(restaurant.Id);

            return View(reservationViewModel);
        }


        [HttpGet]
        [Route("Admin/Manager")]
        public async Task<IActionResult> Manager()
        {
            // One Manager per restaurant
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/admin/manager")))
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _userService.GetRestaurantWhereUserWorksIn(user);
            ManagerViewModel managerViewModel = new ManagerViewModel();

            if (restaurant == null)
            {
                managerViewModel.Error = _manager_restaurant_error;
                return View(managerViewModel);
            }

            managerViewModel.Staff = user;
            managerViewModel.Restaurant = restaurant;
            managerViewModel.Employees = await _restaurantService
                .GetRestaurantEmployeesByRestaurantId(restaurant.Id);

            return View(managerViewModel);
        }


        [HttpGet]
        [Route("Admin/Delivery")]
        public async Task<IActionResult> Delivery()
        {
            // Delivery people can take orders from different restaurants in their own city.
            // They just have to update their profile in order to change which restaurants they can take 
            // orders and deliver them
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/admin/delivery")))
            {
                return RedirectToAction("Login", "User");
            }

            Dictionary<TimeTableModel, List<OrderModel>> orders = new Dictionary<TimeTableModel, List<OrderModel>>();

            foreach (TimeTableModel timeTable in await _restaurantService.GetRestaurantsForDelivery_ForUser(user)) {

                orders.Add(timeTable, await _orderService
                    .GetOrdersByRestaurantId_WithHomeDeliveryOption(timeTable.Restuarant.Id, true));
            }

            DeliveryViewModel deliveryViewModel = new DeliveryViewModel()
            {
                Staff = user,
                Orders = orders
            };

            return View(deliveryViewModel);
        }
    }
}
