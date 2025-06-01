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
        public static readonly string RestaurantNotFountError = "Can't determin in which restaurant you work in. Please contact your manager.";

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
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            return user == null ? RedirectToAction("Login", "User") : View(user);
        }

    }
}
