using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Models.View.Admin;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers
{

    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class StaffController : Controller
    {
        private readonly string _restaurant_error = "Can't determin in which restaurant you work in. Please contact your manager.";

        private OrderService _orderService;
        private UserUtility _userUtils;
        private RestaurantService _restaurantService;
        private OrderedDishesService _orderedDishesService;
        private ReservationService _reservationService;
        private UserService _userService;
        private WebSocketService _webSocketService;

        public StaffController(UserUtility userUtils,
            Utility _utils, OrderService orderService, 
            RestaurantService restaurantService,
            OrderedDishesService orderedDishesService, 
            ReservationService reservationService,
            UserService userService, WebSocketService webSocketService)
        {
            _userUtils = userUtils;
            _orderService = orderService;
            _restaurantService = restaurantService;
            _orderedDishesService = orderedDishesService;
            _reservationService = reservationService;
            _userService = userService;
            _webSocketService = webSocketService;
        }

        [HttpGet]
        [Route("/staff")]
        [Route("/staff/index")]
        public async Task<IActionResult> Index()
        {
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            return user == null ? RedirectToAction("Login", "User") : View(user);
        }


        [HttpGet]
        [Route("/staff/dishes")]
        public async Task<IActionResult> Dishes()
        {
            // Chief in the kitchen

            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _userService.GetRestaurantWhereUserWorksIn(user);

            if (restaurant == null)
            {
                TempData["Error"] = _restaurant_error;
                return View();
            }

            List<OrderWithDishesCountModel> dishes = new ();
            foreach (OrderModel order in await _orderService.GetOrdersByRestaurantId(restaurant.Id))
            {
                dishes.Add(new OrderWithDishesCountModel() { 
                    Order = order,
                    DishesCount = await _orderedDishesService.CountDishesByOrder(order.Id)
                });
            }

            return View(new DishesViewModel()
            {
                Restaurant = restaurant,
                Staff = user,
                OrderWithDishesCount = dishes
            });
        }


        [HttpPost]
        [Route("/staff/dishes")]
        public async Task<IActionResult> DishesUpdate([FromForm] OrderUpdateFormModel orderUpdateFormModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }

            string updateMessage = "Updated ";

            if (orderUpdateFormModel.OrderCurrentStatus != null)
            {
                if (!(await _orderService.UpdateOrderCurrentStatusById((int)orderUpdateFormModel.OrderId,
                orderUpdateFormModel.OrderCurrentStatus)))
                {
                    return BadRequest();
                }

                updateMessage += "order's current status: " + orderUpdateFormModel.DishCurrentStatus+". ";
            }

            if (orderUpdateFormModel.DishCurrentStatus != null &&
                orderUpdateFormModel.DishId != null)
            {

                if(!(await _orderedDishesService.UpdateOrderedDishStatusById((int)orderUpdateFormModel.DishId,
                    orderUpdateFormModel.OrderId, orderUpdateFormModel.DishCurrentStatus)))
                {
                    return BadRequest();
                }

                updateMessage += "dish current status: " + orderUpdateFormModel.DishCurrentStatus+". ";
            }

            if (updateMessage.Length <= 8)
            {
                return BadRequest();
            }

            await _webSocketService.SendJsonToClients("/ws/orders", orderUpdateFormModel,
                     _orderService.GetListenersForOrderId(orderUpdateFormModel.OrderId));

            return Ok(updateMessage);
        }


        [HttpGet]
        [Route("/staff/reservations")]
        public async Task<IActionResult> Reservations()
        {
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _userService.GetRestaurantWhereUserWorksIn(user);

            if (restaurant == null)
            {
                TempData["Error"] = _restaurant_error;
                return View();
            }

            return View(new StaffReservationViewModel()
            {
                Staff = user,
                Reservations = await _reservationService
                .GetReservationsByRestaurantId(restaurant.Id)
            });
        }


        [HttpGet]
        [Route("/staff/manager")]
        public async Task<IActionResult> Manager()
        {
            // One Manager per restaurant
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _userService.GetRestaurantWhereUserWorksIn(user);

            if (restaurant == null)
            {
                TempData["Error"] = _restaurant_error;
                return View();
            }

            return View(new ManagerViewModel() {
                Staff = user,
                Restaurant = restaurant,
                Employees = await _restaurantService
                    .GetRestaurantEmployeesByRestaurantId(restaurant.Id)
            });
        }


        [HttpGet]
        [Route("/staff/delivery")]
        public async Task<IActionResult> Delivery()
        {
            // Delivery people can take orders from different restaurants in their own city.
            // They just have to update their profile in order to change which restaurants they can take 
            // orders and deliver them
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null)
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
