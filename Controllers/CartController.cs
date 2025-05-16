using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.View.User;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers
{

    [ApiController]
    [EnableRateLimiting("fixed")]
    public class CartController : Controller
    {
        private UserUtility _userUtility;
        private DishService _dishService;
        private RestaurantService _restaurantService;

        public CartController(DishService dishService, UserUtility userUtility, 
            RestaurantService restaurantService)
        {
            _dishService = dishService;
            _userUtility = userUtility;
            _restaurantService = restaurantService;
        }


        [HttpGet]
        [Route("/cart")]
        [Route("/cart/index")]
        public async Task<IActionResult> Index()
        {

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            return View(new CartViewModel()
            {
                User = user,
                Restaurant = restaurant,
                Dishes = await _dishService.GetDishesByIds(
                _dishService.GetDishIDsFromCart(HttpContext))
            });
        }
    }
}
