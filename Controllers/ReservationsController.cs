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
        private static readonly string _forbit = "Please select a restaurant, that serves customers on-site";
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
        public async Task<IActionResult> Reservations()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantByIdAsync(
                _restaurantService.GetRestaurantIdFromCookieHeaderAsync(HttpContext));

            if (restaurant == null || !restaurant.ServeCustomersInPlace)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            return View(new ReservationsViewModel()
            {
                User = user,
                Reservations = await _reservationService.GetReservationsByUserIdAsync(user.Id)
            });
        }
        

        [HttpGet]
        [Route("/reservation")]
        public async Task<IActionResult> Reservation()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantByIdAsync(
                _restaurantService.GetRestaurantIdFromCookieHeaderAsync(HttpContext));

            if (restaurant == null || !restaurant.ServeCustomersInPlace)
            {
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
            if (!ModelState.IsValid)
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
                TempData["Message"] = _forbit;
                return RedirectToAction("Index", "Restaurant");
            }

            if (await _reservationService.CreateReservationAsync(user.Id, restaurant.Id,
                reservationFormModel.Amount_Of_Adults, reservationFormModel.Amount_Of_Children,
                reservationFormModel.At_Date, reservationFormModel.Notes)
                != null)
            {
                TempData["ReservationSuccessfull"] = true;
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
                TempData["CanceledSuccessfull"] = true;
                return Ok();
            }

            return BadRequest();
        }
    }
}
