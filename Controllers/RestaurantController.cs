using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers
{


    [ApiController]
    [EnableRateLimiting("fixed")]
    public class RestaurantController : Controller
    {

        private UserUtility _userUtility;
        private RestaurantService _restaurantService;
        private readonly string restaurant_id = "restaurant_id";

        public RestaurantController(UserUtility userUtility, 
            RestaurantService restaurantService) {
            _userUtility = userUtility;
            _restaurantService = restaurantService;
        }


        [HttpGet]
        [Route("Restaurants")]
        public async Task<IActionResult> Index()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(await _restaurantService.GetAllRestaurantsForUser(user));
        }

        [HttpPost]
        [Route("Restaurant")]
        [IgnoreAntiforgeryToken]
        public IActionResult Restaurant([FromForm] int restaurantId)
        {
            HttpContext.Response.Cookies.Delete(restaurant_id);
            HttpContext.Response.Cookies.Append(restaurant_id, restaurantId.ToString());
            return RedirectToAction("Dishes", "Dish");
        }
    }
}
