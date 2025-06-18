using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers.Staff
{

    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class StaffController : Controller
    {
        private UserUtility _userUtils;

        public StaffController(UserUtility userUtils)
        {
            _userUtils = userUtils;
        }

        [HttpGet]
        [Route("/staff")]
        [Route("/staff/index")]
        public async Task<IActionResult> Index()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            return user == null ? RedirectToAction("Login", "User") : View(user);
        }

    }
}
