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
        private CuponService _cuponService;
        private RestaurantService _restaurantService;

        public OrderController(OrderService orderService, 
            DishService dishService, UserUtility userUtility,
            RestaurantService restaurantService, CuponService cuponService)
        {
            _orderService = orderService;
            _dishService = dishService;
            _userUtility = userUtility;
            _restaurantService = restaurantService;
            _cuponService = cuponService;
        }

        [HttpGet]
        [Route("/orders")]
        [Route("/orders/index")]
        public async Task<IActionResult> Orders()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            Dictionary<OrderModel, List<DishModel>> orders = new Dictionary<OrderModel, List<DishModel>>();

            foreach (OrderModel order in await _orderService.GetOrdersByUser(user.Id))
            {
                orders.Add(order, await _dishService.GetDishesByIds(
                    order.OrderedDishes.Select(dish => dish.DishId).ToList()));
            }

            return View("Orders", new OrdersViewModel()
            {
                User = user,
                Orders = orders
            });
        }


        [HttpGet]
        [Route("/order/{orderId}")]
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
        [Route("/order/start")]
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

            decimal totalPrice = (await _dishService.GetDishesByIds(order.Dishes)).Sum(dish => dish.Price);

            if (order.CuponCode.Replace(" ", "").Length > 0) {
                CuponModel? cupon = await _cuponService.GetCuponByCode(order.CuponCode);
                if (cupon != null)
                {
                    totalPrice = _cuponService.HandleCuponDiscount(cupon.DiscountPercent, totalPrice);
                }
            }

            return View(new OrderStartViewModel()
            {
                User = user,
                Restaurant = restaurant,
                Order = await _orderService.AddOrder(user.Id, restaurant.Id,
                order.Dishes, order.Notes, totalPrice, null, order.CuponCode)
            });
        }


        [HttpPost]
        [Route("/order/stop/{orderId}")]
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

            return View(new OrderStopView()
            {
                User = user,
                Success = await _orderService.DeleteOrder(orderId)
            });
        }
    }
}
