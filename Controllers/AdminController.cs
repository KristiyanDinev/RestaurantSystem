using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.View;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers
{

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

        public AdminController(UserUtility userUtils,
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
        [EnableRateLimiting("fixed")]
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
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Dishes()
        {
            // Chief in the kitchen

            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/admin/Dishes")))
            {
                return Redirect("/login");
            }

            RestaurantModel? restaurant = await _userSerive.(user);

            DishesViewModel adminDishesModel = new DishesViewModel();
            if (restaurant == null)
            {
                adminDishesModel.Error = _restaurant_error;
                return View(adminDishesModel);
            }

            Dictionary<OrderModel, List<DishModel>> dishes = new Dictionary<OrderModel, List<DishModel>>();
            foreach (OrderModel order in await _orderService.GetOrdersByRestaurantId(restaurant.Id))
            {
                dishes.Add(order, await _orderedDishesService.GetDishesFromOrder(order.Id));
            }

            adminDishesModel.Restaurant = restaurant;
            adminDishesModel.Staff = user;
            adminDishesModel.Dishes = dishes;

            return View(adminDishesModel);
        }


        [HttpGet]
        [Route("Admin/Reservations")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Reservations()
        {
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/admin/Dishes")))
            {
                return Redirect("/login");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantForStaff(user);
            ReservationViewModel reservationViewModel = new ReservationViewModel();

            if (restaurant == null)
            {
                reservationViewModel.Error = _restaurant_error;
                return View(reservationViewModel);
            }

            reservationViewModel.Staff = user;
            reservationViewModel.Reservations = await _reservationService.GetReservationsByRestaurantId(restaurant.Id);
            return View(reservationViewModel);
        }


        [HttpGet]
        [Route("Admin/Manager")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Manager()
        {
            // One Manager per restaurant
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/admin/manager")))
            {
                return Redirect("/login");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantForStaff(user);
            ManagerViewModel managerViewModel = new ManagerViewModel();

            if (restaurant == null)
            {
                managerViewModel.Error = _manager_restaurant_error;
                return View(managerViewModel);
            }

            managerViewModel.Staff = user;
            managerViewModel.Restaurant = restaurant;
            managerViewModel.Employees = await _roleService.GetUsersWithAccessToServicesInTheRestaurant(
                new List<string> { "/admin/Dishes", "/admin/Reservations" },
                restaurant);

            return View(managerViewModel);
        }


        [HttpGet]
        [Route("Admin/Delivery")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Delivery()
        {
            // Delivery people can take orders from different restaurants
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null ||
                !(await _roleService.CanUserAccessService(user.Id, "/admin/delivery")))
            {
                return Redirect("/login");
            }

            DeliveryViewModel deliveryViewModel = new DeliveryViewModel();

            Dictionary<TimeTableModel, List<OrderModel>> orders = new Dictionary<TimeTableModel, List<OrderModel>>();

            foreach (TimeTableModel timeTable in await _restaurantService.GetRestaurantsForDelivery_ForUser(user)) {

                orders.Add(timeTable, await _orderService.GetOrdersByRestaurantId_WithHomeDeliveryOption(timeTable.Restuarant.Id, true));
            }

            deliveryViewModel.Staff = user;
            deliveryViewModel.Orders = orders;

            return View(deliveryViewModel);
        }
    }
}
