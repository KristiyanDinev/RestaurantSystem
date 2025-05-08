using RestaurantSystem.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Services;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Utilities;
using RestaurantSystem.Models.View.Order;
using RestaurantSystem.Models.Form;

namespace RestaurantSystem.Controllers {
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
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Orders()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return Redirect("/login");
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
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Order(int orderId)
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return Redirect("/login");
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
        [EnableRateLimiting("fixed")]
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
                return Redirect("/login");
            }

            OrderStartViewModel orderStartViewModel = new OrderStartViewModel()
            {
                User = user,
                Restaurant = restaurant
            };

            try
            {
                OrderModel orderModel = await _orderService.AddOrder(user.Id, restaurant.Id, order.Dishes, order.Notes,

                (await _dishService.GetDishesByIds(order.Dishes)).Sum(dish => dish.Price)

                , true);

                orderStartViewModel.Success = true;
                orderStartViewModel.Order = orderModel;

                return View(orderStartViewModel);

            } catch (Exception e)
            {
                orderStartViewModel.Success = false;
                return View(orderStartViewModel);
            }
        }


        [HttpPost]
        [Route("Order/Stop/{orderId}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> OrderStop(int orderId)
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
                return Redirect("/login");
            }

            OrderStopView orderStopView = new OrderStopView()
            {
                User = user
            };

            try
            {
                await _orderService.DeleteOrder(orderId);

                orderStopView.Success = true;

                return View(orderStopView);

            }
            catch (Exception e)
            {
                orderStopView.Success = false;
                return View(orderStopView);
            }
        }
    }
}
