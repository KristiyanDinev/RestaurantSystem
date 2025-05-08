using RestaurantSystem.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Services;
using RestaurantSystem.Models.View.Dish;

namespace RestaurantSystem.Controllers {



    [ApiController]
    [EnableRateLimiting("fixed")]
    public class DishController : Controller {

        private DishService _dishService;
        private RestaurantService _restaurantService;

        public DishController(RestaurantService restaurantService, DishService dishService) {
            _restaurantService = restaurantService;
            _dishService = dishService;
        }


        [HttpGet]
        [Route("/Dishes")]
        [Route("/Dishes/Index")]
        public async Task<IActionResult> Dishes()
        {
            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null) {
                return RedirectToAction("Index", "Restaurant");
            }

            return View(restaurant);
        }


        [HttpGet]
        [Route("/Dishes/{dishType}")]
        public async Task<IActionResult> DishesByType(string dishType)
        {
            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            DishesTypeViewModel dishesTypeViewModel = new DishesTypeViewModel()
            {
                Dishes = await _dishService.GetDishesByTypeAndRestaurantId(
                    dishType, restaurant.Id),
                Restaurant = restaurant
            };

            return View(dishesTypeViewModel);
        }


        [HttpGet]
        [Route("/Dish/{dishId}")]
        public async Task<IActionResult> DishesByType(int dishId)
        {
            Console.WriteLine("Dish Id: " + dishId);
            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            DishViewModel dishViewModel = new DishViewModel()
            {
                Dish = await _dishService.GetDishById(dishId),
                Restaurant = restaurant
            };

            return View(dishViewModel);
        }

    }
}
