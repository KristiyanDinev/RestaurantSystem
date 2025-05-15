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
    public class StaffController : Controller
    {
        private readonly string _restaurant_error = "Can't determin in which restaurant you work in. Please contact your manager.";

        private RoleService _roleService;
        private OrderService _orderService;
        private UserUtility _userUtils;
        private RestaurantService _restaurantService;
        private OrderedDishesService _orderedDishesService;
        private ReservationService _reservationService;
        private UserService _userService;
        private WebSocketService _webSocketService;

        public StaffController(UserUtility userUtils,
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
        [Route("Staff")]
        [Route("Staff/Index")]
        public async Task<IActionResult> Index()
        {
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/staff")))
            {
                return RedirectToAction("Login", "User");
            }

            return View(user);
        }


        [HttpGet]
        [Route("Staff/Dishes")]
        public async Task<IActionResult> Dishes()
        {
            // Chief in the kitchen

            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/staff/dishes")))
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
        [Route("Staff/Dishes")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DishesUpdate([FromForm] OrderUpdateFormModel orderUpdateFormModel)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ErrorMessage"] = "Invalid form submission";
                return RedirectToAction("Dishes");
            }

            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/staff/dishes")))
            {
                return RedirectToAction("Login", "User");
            }

            string updateMessage = "Updated ";

            if (orderUpdateFormModel.OrderCurrentStatus != null)
            {
                if (!(await _orderService.UpdateOrderCurrentStatusById((int)orderUpdateFormModel.OrderId,
                orderUpdateFormModel.OrderCurrentStatus)))
                {
                    ViewData["ErrorMessage"] = "Can't update order's current status.";
                    return View("Dishes");
                }

                updateMessage += "order's current status: " + orderUpdateFormModel.DishCurrentStatus+". ";
            }

            if (orderUpdateFormModel.DishCurrentStatus != null &&
                orderUpdateFormModel.DishId != null)
            {

                if(!(await _orderedDishesService.UpdateOrderedDishStatusById((int)orderUpdateFormModel.DishId,
                    orderUpdateFormModel.OrderId, orderUpdateFormModel.DishCurrentStatus)))
                {
                    ViewData["ErrorMessage"] = "Can't update dish's current status.";
                    return View("Dishes");
                }

                updateMessage += "dish current status: " + orderUpdateFormModel.DishCurrentStatus+". ";
            }

            if (updateMessage.Length > 8)
            {
                ViewData["SuccessMessage"] = updateMessage;
                await _webSocketService.SendJsonToClients("/ws/orders", orderUpdateFormModel,
                     _orderService.GetListenersForOrderId(orderUpdateFormModel.OrderId));
            }
            return RedirectToAction("Dishes");
        }


        [HttpGet]
        [Route("Staff/Reservations")]
        public async Task<IActionResult> Reservations()
        {
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/staff/reservations")))
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _userService.GetRestaurantWhereUserWorksIn(user);
            StaffReservationViewModel reservationViewModel = new StaffReservationViewModel();

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
        [Route("Staff/Manager")]
        public async Task<IActionResult> Manager()
        {
            // One Manager per restaurant
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/staff/manager")))
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _userService.GetRestaurantWhereUserWorksIn(user);
            ManagerViewModel managerViewModel = new ManagerViewModel();

            if (restaurant == null)
            {
                managerViewModel.Error = _restaurant_error;
                return View(managerViewModel);
            }

            managerViewModel.Staff = user;
            managerViewModel.Restaurant = restaurant;
            managerViewModel.Employees = await _restaurantService
                .GetRestaurantEmployeesByRestaurantId(restaurant.Id);

            return View(managerViewModel);
        }


        [HttpGet]
        [Route("Staff/Delivery")]
        public async Task<IActionResult> Delivery()
        {
            // Delivery people can take orders from different restaurants in their own city.
            // They just have to update their profile in order to change which restaurants they can take 
            // orders and deliver them
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/staff/delivery")))
            {
                return RedirectToAction("Login", "User");
            }

            Dictionary<RestaurantModel, List<OrderModel>> orders = new ();

            foreach (RestaurantModel restaurant in await _restaurantService
                .GetDeliveryGuy_Restaurants(user)) {

                orders.Add(restaurant, await _orderService
                    .Get_HomeDelivery_OrdersBy_RestaurantId(restaurant.Id));
            }

            return View(new DeliveryViewModel()
            {
                Staff = user,
                Orders = orders
            });
        }
    }
}
