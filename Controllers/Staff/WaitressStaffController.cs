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
                !Utility.IsValidReservationStatus(reservationUpdateForm.Status))
            {
                return BadRequest();
            }

            return await _reservationService.UpdateReservationAsync(reservationUpdateForm.Id,
                reservationUpdateForm.Status) ? Ok() : BadRequest();
        }


        [HttpPost]
        [Route("/staff/reservations/delete/{id}")]
        public async Task<IActionResult> ReservationDelete(int id)
        {
            ReservationModel? reservation = await _reservationService
                .GetReservationByIdAsync(id);

            if (reservation == null || !reservation.CurrentStatus.Equals(Status.Cancelled.ToString(),
                StringComparison.OrdinalIgnoreCase)) { 
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
        [Route("/staff/order/create")]
        public async Task<IActionResult> OrderCreate()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            int restaurantId = user.Restaurant.Id;
            List<DishModel> salads = await _dishService.GetDishesByTypeAndRestaurantIdAsync("salad", restaurantId);
            List<DishModel> soups = await _dishService.GetDishesByTypeAndRestaurantIdAsync("soup", restaurantId);
            List<DishModel> appetizers = await _dishService.GetDishesByTypeAndRestaurantIdAsync("appetizers", restaurantId);
            List<DishModel> dishes = await _dishService.GetDishesByTypeAndRestaurantIdAsync("dishes", restaurantId);
            List<DishModel> drinks = await _dishService.GetDishesByTypeAndRestaurantIdAsync("drink", restaurantId);
            List<DishModel> desserts = await _dishService.GetDishesByTypeAndRestaurantIdAsync("desserts", restaurantId);

            return View(new OrderCreateViewModel { 
                User = user
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
