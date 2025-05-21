using RestaurantSystem.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Services;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Utilities;
using RestaurantSystem.Models.View.Order;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Models;

namespace RestaurantSystem.Controllers {


    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class OrderController : Controller {

        private OrderService _orderService;
        private DishService _dishService;
        private UserUtility _userUtility;
        private CuponService _cuponService;
        private OrderedDishesService _orderedDishesService;
        private RestaurantService _restaurantService;

        public OrderController(OrderService orderService, 
            DishService dishService, UserUtility userUtility,
            RestaurantService restaurantService, CuponService cuponService, 
            OrderedDishesService orderedDishesService)
        {
            _orderService = orderService;
            _dishService = dishService;
            _userUtility = userUtility;
            _restaurantService = restaurantService;
            _cuponService = cuponService;
            _orderedDishesService = orderedDishesService;
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

            List<OrderWithDishesCountModel> orders = new ();

            foreach (OrderModel order in await _orderService.GetOrdersByUser(user.Id))
            {
                orders.Add(new OrderWithDishesCountModel() {
                    Order = order,
                    DishesCount = await _orderedDishesService.CountDishesByOrder(order.Id)
                });
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

            return View(new OrderViewModel()
            {
                User = user,
                Order = await _orderService.GetOrderById(orderId)
            });
        }


        [HttpPost]
        [Route("/order/start")]
        public async Task<IActionResult> OrderStart([FromForm] OrderFormModel order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null)
            {
                return BadRequest();
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }

            List<int> DishIds = _dishService.GetDishIDsFromCart(HttpContext);

            decimal totalPrice = 0;
            List<int> CoutingDishId = new List<int>(DishIds);

            foreach (DishModel dishModel in await _dishService.GetDishesByIds(DishIds.ToHashSet()))
            {
                int beforeRemovalCount = CoutingDishId.Count;
                CoutingDishId.RemoveAll(id => id == dishModel.Id);

                totalPrice += dishModel.Price * (beforeRemovalCount - CoutingDishId.Count);
            }

            if (order.CuponCode != null && order.CuponCode.Replace(" ", "").Length > 0) {
                CuponModel? cupon = await _cuponService.GetCuponByCode(order.CuponCode);
                if (cupon != null)
                {
                    totalPrice = _cuponService.HandleCuponDiscount(cupon.DiscountPercent, totalPrice);
                }
            }

            if ((await _orderService.AddOrder(user.Id, restaurant.Id,
                DishIds, order.Notes, totalPrice, null, order.CuponCode)) == null)
            {
                return BadRequest();
            }
            HttpContext.Response.Cookies.Delete("cart_items");
            return Ok();
        }


        [HttpPost]
        [Route("/order/stop/{orderId}")]
        public async Task<IActionResult> OrderStop(int orderId)
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }

            if (!(await _orderService.DeleteOrder(orderId)))
            {
                return BadRequest();
            }

            return BadRequest();
        }
    }
}
