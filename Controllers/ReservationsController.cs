﻿using Microsoft.AspNetCore.Mvc;
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
        [Route("/reservation")]
        [Route("/reservation/index")]
        public async Task<IActionResult> Reservation()
        {
            RestaurantModel? restaurant = await _restaurantService.GetRestaurantByIdAsync(
                _restaurantService.GetRestaurantIdFromCookieHeaderAsync(HttpContext));

            if (restaurant == null)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
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

            if (restaurant == null)
            {
                return BadRequest();
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


        [HttpGet]
        [Route("/reservations")]
        public async Task<IActionResult> Reservations()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(new ReservationsViewModel()
            {
                User = user,
                Reservations = await _reservationService.GetReservationsByUserIdAsync(user.Id)
            });
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
                Status.Cancelled.ToString()))
            {
                TempData["CanceledSuccessfull"] = true;
                return Ok();
            }

            return BadRequest();
        }
    }
}
