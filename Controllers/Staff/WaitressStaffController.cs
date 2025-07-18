using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Enums;
using RestaurantSystem.Models;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Models.View.Dish;
using RestaurantSystem.Models.View.Staff.Waitress;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers.Staff
{

    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class WaitressStaffController : Controller
    {
        private UserUtility _userUtility;
        private ReservationService _reservationService;
        private OrderService _orderService;
        private OrderedDishesService _orderedDishesService;
        private DishService _dishService;
        private CuponService _cuponService;
        private WebSocketService _webSocketService;

        public WaitressStaffController(UserUtility userUtility,
            OrderedDishesService orderedDishesService,
            ReservationService reservationService,
            OrderService orderService,
            DishService dishService,
            CuponService cuponService,
            WebSocketService webSocketService)
        {
            _userUtility = userUtility;
            _reservationService = reservationService;
            _orderService = orderService;
            _orderedDishesService = orderedDishesService;
            _dishService = dishService;
            _cuponService = cuponService;
            _webSocketService = webSocketService;
        }

        [HttpGet]
        [Route("/staff/reservations")]
        public async Task<IActionResult> Reservations()
        {
            UserModel? user = await _userUtility.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(new ReservationViewModel()
            {
                Staff = user,
                Reservations = await _reservationService
                .GetReservationsByRestaurantIdAsync(user.Restaurant.Id)
            });
        }


        [HttpPost]
        [Route("/staff/reservations")]
        public async Task<IActionResult> ReservationUpdate(
            [FromForm] ReservationUpdateFormModel reservationUpdateForm)
        {
            if (!ModelState.IsValid || 
                !ReservationStatusEnum.TryParse(reservationUpdateForm.Status, true, out
                    ReservationStatusEnum status))
            {
                return BadRequest();
            }

            return await _reservationService.UpdateReservationAsync(
                reservationUpdateForm.Id, status) ? Ok() : BadRequest();
        }


        [HttpPost]
        [Route("/staff/reservations/delete/{id}")]
        public async Task<IActionResult> ReservationDelete(int id)
        {
            ReservationModel? reservation = await _reservationService
                .GetReservationByIdAsync(id);
            if (reservation == null || !reservation.CurrentStatus.Equals(ReservationStatusEnum.Cancelled)) { 
                return BadRequest();
            }
            
            return await _reservationService.DeleteReservationAsync(reservation.Id) ?
                Ok() : BadRequest();
        }


        [HttpPost]
        [Route("/staff/orders/serve/{orderId}")]
        public async Task<IActionResult> OrderServePost(long orderId, [FromForm] bool IsServed)
        {
            UserModel? user = await _userUtility.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            OrderStatusEnum statusEnum = IsServed ? OrderStatusEnum.Served : OrderStatusEnum.Ready;
            if (await _orderService.UpdateOrderCurrentStatusByIdAsync(orderId, statusEnum))
            {
                await _webSocketService.SendJsonToOrder("/ws/orders", orderId, new OrderUpdateFormModel()
                {
                    DishId = 0,
                    OrderId = orderId,
                    OrderCurrentStatus = statusEnum.ToString()
                });

                if (IsServed)
                {
                    TempData["ServedOrderSuccess"] = true;
                }
                else
                {
                    TempData["UnservedOrderSuccess"] = true;
                }
                return Ok();
            }
            return BadRequest();
        }


        [HttpGet]
        [Route("/staff/orders")]
        public async Task<IActionResult> Orders()
        {
            UserModel? user = await _userUtility.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            List<OrderWithDishesCountModel> dishes = new();
            foreach (OrderModel order in await _orderService.GetWaitressOrdersByRestaurantIdAsync(user.Restaurant.Id))
            {
                dishes.Add(new OrderWithDishesCountModel()
                {
                    Order = order,
                    DishesCount = await _orderedDishesService.CountDishesByOrderAsync(order.Id)
                });
            }

            return View(new OrdersViewModel()
            {
                Staff = user,
                Orders = dishes
            });
        }


        [HttpGet]
        [Route("/staff/orders/details")]
        public async Task<IActionResult> OrderDetails()
        {
            UserModel? user = await _userUtility.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            List<int> DishIds = _dishService.GetDishIDsFromCartAsync(HttpContext);
            HashSet<int> hash_ids = new(DishIds);
            Dictionary<DishModel, int> dishes = new();
            List<DishModel> dishModels = await _dishService.GetDishesByIdsAsync(hash_ids);

            foreach (int eachDishId in hash_ids)
            {
                int countOfItems = DishIds.Count;
                DishIds.RemoveAll(id => id == eachDishId);

                DishModel? dish = dishModels.FirstOrDefault(dish => dish.Id == eachDishId);

                if (dish != null)
                {
                    dishes.Add(dish, countOfItems - DishIds.Count);
                }
            }
            return View(new OrderDetailsViewModel { 
                Staff = user,
                Dishes = dishes,
            });
        }


        [HttpGet]
        [Route("/staff/orders/dishes")]
        public async Task<IActionResult> OrderDishes()
        {
            UserModel? user = await _userUtility.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View(user);
        }


        [HttpGet]
        [Route("/staff/orders/dishes/{dishTypeStr}")]
        public async Task<IActionResult> OrderDishByType(string dishTypeStr)
        {
            if (!DishTypeEnum.TryParse(dishTypeStr, true, out DishTypeEnum dishType))
            {
                return BadRequest();
            }
            UserModel? user = await _userUtility.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(new DishesTypeViewModel()
            {
                User = user,
                Dishes = await _dishService.GetDishesByTypeAndRestaurantIdAsync(dishType, user.Restaurant.Id),
                DishType = dishType.ToString(),
                Restaurant = user.Restaurant
            });
        }


        [HttpPost]
        [Route("/staff/orders/details")]
        public async Task<IActionResult> OrderDetailsPost(
            [FromForm] WaitressOrderFormModel waitressOrderForm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            UserModel? user = await _userUtility.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            List<int> DishIds = _dishService.GetDishIDsFromCartAsync(HttpContext);

            decimal totalPrice = 0;
            List<int> CountingDishId = new List<int>(DishIds);
            foreach (DishModel dishModel in await _dishService.GetDishesByIdsAsync(DishIds.ToHashSet()))
            {
                int beforeRemovalCount = CountingDishId.Count;
                CountingDishId.RemoveAll(id => id == dishModel.Id);

                totalPrice += dishModel.Price * (beforeRemovalCount - CountingDishId.Count);
            }

            string? validCupon = null;
            if (!string.IsNullOrWhiteSpace(waitressOrderForm.CuponCode))
            {
                CuponModel? cupon = await _cuponService.GetCuponByCodeAsync(waitressOrderForm.CuponCode);
                if (cupon != null)
                {
                    totalPrice = _cuponService.HandleCuponDiscount(cupon.DiscountPercent, totalPrice);
                    validCupon = waitressOrderForm.CuponCode;
                }
            }

            if ((await _orderService.AddOrderAsync(user.Id, user.Restaurant.Id,
                DishIds, waitressOrderForm.Notes, totalPrice, waitressOrderForm.TableNumber,
                validCupon, null)) == null)
            {
                return BadRequest();
            }

            _userUtility.RemoveCartCookie(HttpContext);
            TempData["OrderedSuccess"] = true;
            return Ok();
        }


        [HttpPost]
        [Route("/staff/orders/delete/{orderId}")]
        public async Task<IActionResult> OrderDeletePost(long orderId)
        {
            UserModel? user = await _userUtility.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (await _orderService.DeleteOrderAsync(orderId, false))
            {
                TempData["DeletedOrderSuccess"] = true;
                return Ok();
            }
            return BadRequest();
        }
    }
}
