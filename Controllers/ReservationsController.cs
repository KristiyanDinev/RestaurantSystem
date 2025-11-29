using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;
using RestaurantSystem.Models.Form;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.View.Reservation;
using RestaurantSystem.Enums;

namespace RestaurantSystem.Controllers
{


    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class ReservationsController : Controller
    {
        private ReservationService _reservationService;
        private UserUtility _userUtility;
        private RestaurantService _restaurantService;

        public ReservationsController(ReservationService reservationService,
            UserUtility userUtility, RestaurantService restaurantService)
        {
            _reservationService = reservationService;
            _userUtility = userUtility;
            _restaurantService = restaurantService;
        }


        [HttpGet]
        [Route("/reservations")]
        [Route("/reservations/index")]
        public async Task<IActionResult> Reservations([FromQuery] int page = 1)
        {
            UserModel? user = await _userUtility.GetUserWithRolesByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantByIdAsync(
                _restaurantService.GetRestaurantIdFromCookieHeaderAsync(HttpContext));

            if (restaurant == null || !restaurant.ServeCustomersInPlace)
            {
                TempData["ReservationInvalidRestaurant"] = true;
                return RedirectToAction("Index", "Restaurant");
            }

            return View(new ReservationsViewModel()
            {
                User = user,
                Page = page,
                Reservations = await _reservationService.GetReservationsByUserIdAsync(user.Id, page)
            });
        }
        

        [HttpGet]
        [Route("/reservation")]
        public async Task<IActionResult> Reservation()
        {
            UserModel? user = await _userUtility.GetUserWithRolesByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantByIdAsync(
                _restaurantService.GetRestaurantIdFromCookieHeaderAsync(HttpContext));

            if (restaurant == null || !restaurant.ServeCustomersInPlace)
            {
                TempData["ReservationInvalidRestaurant"] = true;
                return RedirectToAction("Index", "Restaurant");
            }

            return View(new ReservationFormViewModel()
            {
                User = user,
                Restaurant = restaurant
            });
        }


        [HttpPost]
        [Route("/reservation")]
        public async Task<IActionResult> ReservationCreate(
            [FromForm] ReservationFormModel reservationFormModel)
        {
            if (!ModelState.IsValid || reservationFormModel.At_Date.ToUniversalTime() < DateTime.UtcNow)
            {
                return BadRequest();
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantByIdAsync(
                _restaurantService.GetRestaurantIdFromCookieHeaderAsync(HttpContext));
            if (restaurant == null || !restaurant.ServeCustomersInPlace)
            {
                TempData["ReservationInvalidRestaurant"] = true;
                return RedirectToAction("Index", "Restaurant");
            }

            if (await _reservationService.CreateReservationAsync(user.Id, restaurant.Id, 
                    reservationFormModel) != null)
            {
                TempData["ReservationSuccessful"] = true;
                return Ok();
            }

            return BadRequest();
        }



        [HttpPost]
        [Route("/reservation/cancel/{reservationId}")]
        public async Task<IActionResult> ReservationDelete(int reservationId)
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }

            ReservationModel? reservation = await _reservationService
                .GetReservationByIdAsync(reservationId);

            if (reservation == null || 
                DateTime.UtcNow > reservation.At_Date.ToUniversalTime().AddHours(-1)) { 
                return BadRequest();
            }

            if (await _reservationService.UpdateReservationAsync(reservationId, 
                ReservationStatusEnum.Cancelled))
            {
                TempData["CanceledSuccessful"] = true;
                return Ok();
            }

            return BadRequest();
        }
    }
}
