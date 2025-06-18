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

        public WaitressStaffController(UserUtility userUtils,
            OrderedDishesService orderedDishesService,
            ReservationService reservationService)
        {
            _userUtils = userUtils;
            _reservationService = reservationService;
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

            return View(new OrdersViewModel
            {
                Staff = user,
                Orders = new List<OrderWithDishesCountModel>()
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

            return View(user);
        }
    }
}
