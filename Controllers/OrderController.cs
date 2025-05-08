using RestaurantSystem.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Services;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Utilities;
using RestaurantSystem.Models.View.Order;
using RestaurantSystem.Models.Form;

namespace RestaurantSystem.Controllers {


    [ApiController]
    [EnableRateLimiting("fixed")]
    public class OrderController : Controller {

        private OrderService _orderService;
        private DishService _dishService;
        private UserUtility _userUtility;
        private RestaurantService _restaurantService;

        public OrderController(OrderService orderService, 
            DishService dishService, UserUtility userUtility,
            RestaurantService restaurantService) {
            _orderService = orderService;
            _dishService = dishService;
            _userUtility = userUtility;
            _restaurantService = restaurantService;
        }

        [HttpGet]
        [Route("Orders")]
        [Route("Orders/Index")]
        public async Task<IActionResult> Orders()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            OrdersViewModel ordersViewModel = new OrdersViewModel()
            {
                User = user,
                Orders = await _orderService.GetOrdersByUser(user.Id)
            };

            return View(ordersViewModel);
        }


        [HttpGet]
        [Route("Order/{orderId}")]
        public async Task<IActionResult> Order(int orderId)
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            OrderViewModel orderViewModel = new OrderViewModel()
            {
                User = user,
                Order = await _orderService.GetOrderById(orderId)
            };

            return View(orderViewModel);
        }


        [HttpPost]
        [Route("Order/Start")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OrderStart([FromBody] OrderFormModel order)
        {
            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null)
            {
                return Redirect("/restaurants");
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            OrderStartViewModel orderStartViewModel = new OrderStartViewModel()
            {
                User = user,
                Restaurant = restaurant,
                Order = await _orderService.AddOrder(user.Id, restaurant.Id, order.Dishes, order.Notes,

                (await _dishService.GetDishesByIds(order.Dishes)).Sum(dish => dish.Price)

                , true)
            };

            return View(orderStartViewModel);
        }


        [HttpPost]
        [Route("Order/Stop/{orderId}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OrderStop(int orderId)
        {
            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            OrderStopView orderStopView = new OrderStopView()
            {
                User = user,
                Success = await _orderService.DeleteOrder(orderId)
            };

            return View(orderStopView);
        }
    }
}
