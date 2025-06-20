using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Enums;
using RestaurantSystem.Models;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Models.View.Staff.Cook;
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
        private OrderedDishesService _orderedDishesService;
        private WebSocketService _webSocketService;
        private WebSocketUtility _webSocketUtility;

        public CookStaffController(UserUtility userUtils,
            OrderService orderService,
            OrderedDishesService orderedDishesService,
            UserService userService, WebSocketService webSocketService,
            WebSocketUtility webSocketUtility)
        {
            _userUtils = userUtils;
            _orderService = orderService;
            _orderedDishesService = orderedDishesService;
            _webSocketService = webSocketService;
            _webSocketUtility = webSocketUtility;
        }

        [HttpGet]
        [Route("/staff/dishes")]
        public async Task<IActionResult> Dishes()
        {
            // Chief in the kitchen

            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            List<OrderWithDishesCountModel> dishes = new();
            foreach (OrderModel order in await _orderService.GetOrdersByRestaurantIdAsync(user.Restaurant.Id))
            {
                dishes.Add(new OrderWithDishesCountModel()
                {
                    Order = order,
                    DishesCount = await _orderedDishesService.CountDishesByOrderAsync(order.Id)
                });
            }

            return View(new DishesViewModel()
            {
                Staff = user,
                Orders = dishes
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

            if (DishStatusEnum.TryParse(orderUpdateFormModel.DishCurrentStatus,
                true, out DishStatusEnum dishCurrentStatus))
            {
                orderUpdateFormModel.DishCurrentStatus = dishCurrentStatus.ToString();
                if (!await _orderedDishesService
                    .UpdateOrderedDishStatusByIdAsync(orderUpdateFormModel.DishId,
                    orderUpdateFormModel.OrderId, dishCurrentStatus))
                {
                    return BadRequest();
                }

                updated = true;
            }

            List<DishStatusEnum> status = await _orderedDishesService
                .GetDishCurrectStatusAsync(orderUpdateFormModel.OrderId);

            OrderStatusEnum orderStatus;
            if (status.All(s => s.Equals(DishStatusEnum.Ready)))
            {
                orderStatus = OrderStatusEnum.Ready;

            }
            else if (status.Any(s => s.Equals(DishStatusEnum.Preparing)))
            {
                orderStatus = OrderStatusEnum.Preparing;

            }
            else
            {
                orderStatus = OrderStatusEnum.Pending;
            }

            updated = updated && await _orderService
                .UpdateOrderCurrentStatusByIdAsync(orderUpdateFormModel.OrderId, orderStatus);

            if (!updated)
            {
                return BadRequest();
            }

            orderUpdateFormModel.OrderCurrentStatus = orderStatus.ToString();

            await _webSocketService.SendJsonToClients("/ws/orders", orderUpdateFormModel,
                     _webSocketUtility.GetListenersForOrderId(orderUpdateFormModel.OrderId));

            return Ok();
        }

    }
}
