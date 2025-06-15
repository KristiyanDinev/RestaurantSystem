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
        private RestaurantService _restaurantService;

        public ManagerStaffController(UserUtility userUtils,
            RestaurantService restaurantService)
        {
            _userUtils = userUtils;
            _restaurantService = restaurantService;
        }


        [HttpGet]
        [Route("/staff/manager")]
        public async Task<IActionResult> Manager()
        {
            // One Manager per restaurant
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(new ManagerViewModel()
            {
                Staff = user,
                Employees = await _restaurantService
                    .GetRestaurantEmployeesByRestaurantIdAsync(user.Restaurant.Id)
            });
        }
    }
}
