using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Enums;
using RestaurantSystem.Models;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Models.View.Staff;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers.Staff
{
    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class CookStaffController : Controller
    {
        private OrderService _orderService;
        private UserUtility _userUtils;
        private RestaurantService _restaurantService;
        private OrderedDishesService _orderedDishesService;
        private UserService _userService;
        private WebSocketService _webSocketService;
        private WebSocketUtility _webSocketUtility;
        public CookStaffController(UserUtility userUtils,
            OrderService orderService,
            RestaurantService restaurantService,
            OrderedDishesService orderedDishesService,
            UserService userService, WebSocketService webSocketService,
            WebSocketUtility webSocketUtility)
        {
            _userUtils = userUtils;
            _orderService = orderService;
            _restaurantService = restaurantService;
            _orderedDishesService = orderedDishesService;
            _userService = userService;
            _webSocketService = webSocketService;
            _webSocketUtility = webSocketUtility;
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
                TempData["Error"] = StaffController.RestaurantNotFountError;
                return View();
            }

            List<OrderWithDishesCountModel> dishes = new();
            foreach (OrderModel order in await _orderService.GetOrdersByRestaurantId(restaurant.Id))
            {
                dishes.Add(new OrderWithDishesCountModel()
                {
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
        public async Task<IActionResult> DishesUpdate(
            [FromForm] OrderUpdateFormModel orderUpdateFormModel)
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

            bool updated = false;

            if (orderUpdateFormModel.DishCurrentStatus != null)
            {

                if (!await _orderedDishesService
                    .UpdateOrderedDishStatusById(orderUpdateFormModel.DishId,
                    orderUpdateFormModel.OrderId, orderUpdateFormModel.DishCurrentStatus))
                {
                    return BadRequest();
                }

                updated = true;
            }

            List<string> status = await _orderedDishesService
                .GetDishCurrectStatus(orderUpdateFormModel.OrderId);

            string orderStatus;

            string ready = Status.Ready.ToString();
            string preparing = Status.Preparing.ToString();

            if (status.All(s => s.Equals(ready, StringComparison.OrdinalIgnoreCase)))
            {
                orderStatus = ready;

            }
            else if (status.Any(s => s.Equals(preparing, StringComparison.OrdinalIgnoreCase)))
            {
                orderStatus = preparing;

            }
            else
            {
                orderStatus = Status.Pending.ToString();
            }

            updated = updated && await _orderService
                .UpdateOrderCurrentStatusById(orderUpdateFormModel.OrderId, orderStatus);

            if (!updated)
            {
                return BadRequest();
            }

            orderUpdateFormModel.OrderCurrentStatus = orderStatus;

            await _webSocketService.SendJsonToClients("/ws/orders", orderUpdateFormModel,
                     _webSocketUtility.GetListenersForOrderId(orderUpdateFormModel.OrderId));

            return Ok();
        }

    }
}
