using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;
using RestaurantSystem.Models.Form;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.View.Reservation;

namespace RestaurantSystem.Controllers
{


    [ApiController]
    [EnableRateLimiting("fixed")]
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
        [Route("Reservation")]
        public async Task<IActionResult> Reservation()
        {
            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            ReservationFormViewModel reservationViewModel = new ReservationFormViewModel()
            {
                User = user,
                Restaurant = restaurant
            };

            return View(reservationViewModel);
        }


        [HttpGet]
        [Route("Reservations")]
        public async Task<IActionResult> Reservations()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            ReservationsViewModel reservationsViewModel = new ReservationsViewModel()
            {
                User = user,
                Reservations = await _reservationService.GetReservationsByUserId(user.Id)
            };

            return View(reservationsViewModel);
        }


        [HttpGet]
        [Route("Reservation/{reservationId}")]
        public async Task<IActionResult> ReservationById(int reservationId) {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            ReservationViewModel reservationViewModel = new ReservationViewModel()
            {
                User = user,
                Reservation = await _reservationService.GetReservationById(reservationId)
            };

            return View(reservationViewModel);
        }


        [HttpPost]
        [Route("Reservation/Create")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ReservationCreate(
            [FromForm] ReservationFormModel reservationFormModel)
        {
            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }
            
            ReservationCreateViewModel reservationCreateViewModel = new ReservationCreateViewModel()
            {
                User = user,
                Restaurant = restaurant,
                Reservation = await _reservationService.CreateReservation(user.Id, restaurant.Id,
                reservationFormModel.Amount_Of_Adults, reservationFormModel.Amount_Of_Children,
                reservationFormModel.At_Date, reservationFormModel.Notes)
            };

            return View(reservationCreateViewModel);
        }


        [HttpPost]
        [Route("Reservation/Delete/{reservationId}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ReservationDelete(int reservationId)
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            ReservationDeleteViewModel reservationDeleteViewModel = new ReservationDeleteViewModel()
            {
                User = user,
                Success = await _reservationService.DeleteReservation(reservationId)
            };

            return View(reservationDeleteViewModel);
        }
    }
}
