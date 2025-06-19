using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Enums;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.View.Dish;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers {



    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class DishController : Controller {

        private DishService _dishService;
        private RestaurantService _restaurantService;
        private UserUtility _userUtility;

        public DishController(RestaurantService restaurantService, DishService dishService, 
            UserUtility userUtility)
        {
            _restaurantService = restaurantService;
            _dishService = dishService;
            _userUtility = userUtility;
        }


        [HttpGet]
        [Route("/dishes")]
        public async Task<IActionResult> Dishes()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantByIdAsync(
                _restaurantService.GetRestaurantIdFromCookieHeaderAsync(HttpContext));

            if (restaurant == null) {
                return RedirectToAction("Index", "Restaurant");
            }

            return View("Dishes", new DishesViewModel() { 
                Restaurant = restaurant,
                User = user
            });
        }


        [HttpGet]
        [Route("/dishes/{dishType}")]
        public async Task<IActionResult> DishesByType(string dishType)
        {
            if (!DishTypeEnum.TryParse(dishType, false, out DishTypeEnum type))
            {
                return RedirectToAction("Dishes");
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantByIdAsync(
                _restaurantService.GetRestaurantIdFromCookieHeaderAsync(HttpContext));

            if (restaurant == null)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            return View(new DishesTypeViewModel()
            {
                Dishes = await _dishService.GetDishesByTypeAndRestaurantIdAsync(
                    type, restaurant.Id),
                Restaurant = restaurant,
                User = user,
                DishType = dishType
            });
        }


        [HttpGet]
        [Route("/dish/{dishId}")]
        public async Task<IActionResult> DishById(int dishId)
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantByIdAsync(
                _restaurantService.GetRestaurantIdFromCookieHeaderAsync(HttpContext));

            if (restaurant == null)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            DishModel? dish = await _dishService.GetDishByIdAsync(dishId);
            if (dish == null)
            {
                return RedirectToAction("Dishes");
            }

            return View(new DishViewModel()
            {
                Dish = dish,
                Restaurant = restaurant,
                User = user
            });
        }

    }
}
