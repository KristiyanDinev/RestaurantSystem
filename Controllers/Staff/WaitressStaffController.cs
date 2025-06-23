using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Enums;
using RestaurantSystem.Models;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
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

        public WaitressStaffController(UserUtility userUtility,
            OrderedDishesService orderedDishesService,
            ReservationService reservationService,
            OrderService orderService,
            OrderedDishesService orderedDishes,
            DishService dishService,
            CuponService cuponService)
        {
            _userUtility = userUtility;
            _reservationService = reservationService;
            _orderService = orderService;
            _orderedDishesService = orderedDishesService;
            _dishService = dishService;
            _cuponService = cuponService;
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
            foreach (OrderModel order in await _orderService.GetOrdersByRestaurantIdAsync(user.Restaurant.Id))
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
        [Route("/staff/orders/create")]
        public async Task<IActionResult> OrderCreate()
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
                Dishs = dishes,
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

            int restaurantId = user.Restaurant.Id;
            return View(new OrderChoseDishViewModel
            { 
                Staff = user,
                Salads = await _dishService.GetDishesByTypeAndRestaurantIdAsync(DishTypeEnum.salads, restaurantId),
                Soups = await _dishService.GetDishesByTypeAndRestaurantIdAsync(DishTypeEnum.soups, restaurantId),
                Appetizers = await _dishService.GetDishesByTypeAndRestaurantIdAsync(DishTypeEnum.appetizers, restaurantId),
                Dishes = await _dishService.GetDishesByTypeAndRestaurantIdAsync(DishTypeEnum.dishes, restaurantId),
                Desserts = await _dishService.GetDishesByTypeAndRestaurantIdAsync(DishTypeEnum.desserts, restaurantId),
                Drinks = await _dishService.GetDishesByTypeAndRestaurantIdAsync(DishTypeEnum.drinks, restaurantId)
            });
        }


        [HttpPost]
        [Route("/staff/orders/create")]
        public async Task<IActionResult> OrderCreatePost(
            [FromForm] WaitressOrderFormModel waitressOrderForm)
        {
            UserModel? user = await _userUtility.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
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

            if (!string.IsNullOrWhiteSpace(waitressOrderForm.CuponCode))
            {
                CuponModel? cupon = await _cuponService.GetCuponByCodeAsync(waitressOrderForm.CuponCode);
                if (cupon != null)
                {
                    totalPrice = _cuponService.HandleCuponDiscount(cupon.DiscountPercent, totalPrice);
                }
            }

            if ((await _orderService.AddOrderAsync(user.Id, user.Restaurant.Id,
                DishIds, waitressOrderForm.Notes, totalPrice, waitressOrderForm.TableNumber,
                waitressOrderForm.CuponCode, null)) == null)
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

            if (await _orderService.DeleteOrderAsync(orderId))
            {
                TempData["DeletedOrderSuccess"] = true;
                return Ok();
            }
            return BadRequest();
        }
    }
}
