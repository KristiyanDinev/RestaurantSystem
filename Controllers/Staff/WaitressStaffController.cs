using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
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
        private UserService _userService;

        public WaitressStaffController(UserUtility userUtils,
            OrderedDishesService orderedDishesService,
            ReservationService reservationService,
            UserService userService)
        {
            _userUtils = userUtils;
            _reservationService = reservationService;
            _userService = userService;
        }

        [HttpGet]
        [Route("/staff/reservations")]
        public async Task<IActionResult> Reservations()
        {
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _userService.GetRestaurantWhereUserWorksIn(user);

            if (restaurant == null)
            {
                TempData["Error"] = StaffController.RestaurantNotFountError;
                return View();
            }

            return View(new StaffReservationViewModel()
            {
                Staff = user,
                Reservations = await _reservationService
                .GetReservationsByRestaurantId(restaurant.Id)
            });
        }
    }
}
