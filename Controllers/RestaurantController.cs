using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers
{


    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class RestaurantController : Controller
    {

        private UserUtility _userUtility;
        private RestaurantService _restaurantService;

        public RestaurantController(UserUtility userUtility, 
            RestaurantService restaurantService) {
            _userUtility = userUtility;
            _restaurantService = restaurantService;
        }


        [HttpGet]
        [Route("/restaurants")]
        public async Task<IActionResult> Index()
        {
            UserModel? user = await _userUtility.GetUserWithRolesByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(await _restaurantService.GetAllRestaurantsForUserAsync(user));
        }
    }
}
