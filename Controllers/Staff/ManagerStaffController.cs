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
    public class ManagerStaffController : Controller
    {
        private UserUtility _userUtils;
        private UserService _userService;
        private RestaurantService _restaurantService;

        public ManagerStaffController(UserUtility userUtils,
            RestaurantService restaurantService,
            UserService userService)
        {
            _userUtils = userUtils;
            _restaurantService = restaurantService;
            _userService = userService;
        }


        [HttpGet]
        [Route("/staff/manager")]
        public async Task<IActionResult> Manager()
        {
            // One Manager per restaurant
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

            return View(new ManagerViewModel()
            {
                Staff = user,
                Restaurant = restaurant,
                Employees = await _restaurantService
                    .GetRestaurantEmployeesByRestaurantId(restaurant.Id)
            });
        }
    }
}
