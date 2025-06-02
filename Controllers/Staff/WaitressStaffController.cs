using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Models.View.Staff;
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
        private Utility _utility;

        public WaitressStaffController(UserUtility userUtils,
            OrderedDishesService orderedDishesService,
            ReservationService reservationService, 
            Utility utility)
        {
            _userUtils = userUtils;
            _reservationService = reservationService;
            _utility = utility;
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

            return View("~/Views/Staff/Reservations.cshtml", new ReservationViewModel()
            {
                Staff = user,
                Reservations = await _reservationService
                .GetReservationsByRestaurantId(user.Restaurant.Id)
            });
        }

        [HttpPost]
        [Route("/staff/reservation")]
        public async Task<IActionResult> ReservationUpdate(
            [FromForm] ReservationUpdateFormModel reservationUpdateForm)
        {
            if (!ModelState.IsValid || 
                !_utility.IsValidReservationStatus(reservationUpdateForm.Status))
            {
                return BadRequest();
            }

            return await _reservationService.UpdateReservation(reservationUpdateForm.Id,
                reservationUpdateForm.Status) ? Ok() : BadRequest();
        }

        [HttpPost]
        [Route("/staff/reservation/delete/{id}")]
        public async Task<IActionResult> ReservationDelete(int id)
        {
            return await _reservationService.DeleteReservation(id) ?
                Ok() : BadRequest();
        }
    }
}
