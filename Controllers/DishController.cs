using RestaurantSystem.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Services;
using RestaurantSystem.Models.View.Dish;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers {



    [ApiController]
    [EnableRateLimiting("fixed")]
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
        [Route("/Dishes")]
        [Route("/Dishes/Index")]
        public async Task<IActionResult> Dishes()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null) {
                return RedirectToAction("Index", "Restaurant");
            }

            return View("Dishes", new DishesViewModel() { 
                Restaurant = restaurant,
                User = user
            });
        }


        [HttpGet]
        [Route("/Dishes/{dishType}")]
        public async Task<IActionResult> DishesByType(string dishType)
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

            dishType = dishType.ToLower();

            return View(new DishesTypeViewModel()
            {
                Dishes = await _dishService.GetDishesByTypeAndRestaurantId(
                    dishType, restaurant.Id),
                Restaurant = restaurant,
                User = user,
                DishType = dishType
            });
        }


        [HttpGet]
        [Route("/Dish/{dishId}")]
        public async Task<IActionResult> DishById(int dishId)
        {
            Console.WriteLine("Dish Id: " + dishId);

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

            DishModel? dish = await _dishService.GetDishById(dishId);
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
