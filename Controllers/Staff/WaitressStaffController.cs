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
        private UserUtility _userUtils;
        private ReservationService _reservationService;
        private OrderService _orderService;
        private OrderedDishesService _orderedDishesService;
        private DishService _dishService;

        public WaitressStaffController(UserUtility userUtils,
            OrderedDishesService orderedDishesService,
            ReservationService reservationService,
            OrderService orderService,
            OrderedDishesService orderedDishes,
            DishService dishService)
        {
            _userUtils = userUtils;
            _reservationService = reservationService;
            _orderService = orderService;
            _orderedDishesService = orderedDishesService;
            _dishService = dishService;
        }

        [HttpGet]
        [Route("/staff/reservations")]
        public async Task<IActionResult> Reservations()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
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
                !ReservationStatusEnum.TryParse(reservationUpdateForm.Status, false, out
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

            return View(new OrdersViewModel
            {
                Staff = user,
                Orders = dishes
            });
        }


        [HttpGet]
        [Route("/staff/orders/create")]
        public async Task<IActionResult> OrderCreate()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            int restaurantId = user.Restaurant.Id;
            return View(new OrderCreateViewModel { 
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
        public async Task<IActionResult> OrderCreatePost()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            /*
            if (await _orderService.AddOrderAsync())
            {
                TempData["OrderedSuccess"] = true;
                return Ok();

            } else
            {
                return BadRequest();
            }*/
            return BadRequest();
        }
    }
}
