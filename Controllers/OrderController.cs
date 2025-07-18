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
            UserModel? user = await _userUtility.GetUserWithRolesByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            List<OrderWithDishesCountModel> orders = new ();
            foreach (OrderModel order in await _orderService.GetOrdersByUserAsync(user.Id))
            {
                orders.Add(new OrderWithDishesCountModel() {
                    Order = order,
                    DishesCount = await _orderedDishesService.CountDishesByOrderAsync(order.Id)
                });
            }

            return View("Orders", new OrdersViewModel()
            {
                User = user,
                Orders = orders
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

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantByIdAsync(
                _restaurantService.GetRestaurantIdFromCookieHeaderAsync(HttpContext));

            if (restaurant == null || !restaurant.DoDelivery)
            {
                return BadRequest();
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }

            List<int> DishIds = _dishService.GetDishIDsFromCartAsync(HttpContext);

            decimal totalPrice = 0;
            List<int> CoutingDishId = new List<int>(DishIds);
            foreach (DishModel dishModel in await _dishService.GetDishesByIdsAsync(DishIds.ToHashSet()))
            {
                int beforeRemovalCount = CoutingDishId.Count;
                CoutingDishId.RemoveAll(id => id == dishModel.Id);

                totalPrice += dishModel.Price * (beforeRemovalCount - CoutingDishId.Count);
            }

            string? validCupon = null;
            if (!string.IsNullOrWhiteSpace(order.CuponCode))
            {
                CuponModel? cupon = await _cuponService.GetCuponByCodeAsync(order.CuponCode);
                if (cupon != null)
                {
                    totalPrice = _cuponService.HandleCuponDiscount(cupon.DiscountPercent, totalPrice);
                    validCupon = order.CuponCode;
                }
            }

            if ((await _orderService.AddOrderAsync(user.Id, restaurant.Id,
                    DishIds, order.Notes, totalPrice, null, validCupon, order.AddressId)) == null)
            {
                    return BadRequest();
            }

            _userUtility.RemoveCartCookie(HttpContext);
            TempData["OrderedSuccess"] = true;
            return Ok();
        }


        [HttpPost]
        [Route("/order/cancel/{orderId}")]
        public async Task<IActionResult> OrderCancele(int orderId)
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }

            if (await _orderService.DeleteOrderAsync(orderId))
            {
                TempData["Canceled"] = true;
                return Ok();
            }

            return BadRequest();
        }
    }
}
