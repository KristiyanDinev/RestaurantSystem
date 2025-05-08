using RestaurantSystem.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Services;
using RestaurantSystem.Models.View.Dish;

namespace RestaurantSystem.Controllers {
    public class DishController : Controller {

        private RestaurantService _restaurantService;
        private DishService _dishService;

        public DishController(RestaurantService restaurantService, 
            DishService dishService) {
            _restaurantService = restaurantService;
            _dishService = dishService;
        }


        [HttpGet]
        [Route("/Dishes")]
        [Route("/Dishes/Index")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Dishes()
        {
            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null) {
                return Redirect("/restaurants");
            }

            return View(restaurant);
        }


        [HttpGet]
        [Route("/Dishes/{dishType}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> DishesByType(string dishType)
        {
            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null)
            {
                return Redirect("/restaurants");
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
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> DishesByType(int dishId)
        {
            Console.WriteLine("Dish Id: " + dishId);
            RestaurantModel? restaurant = await _restaurantService.GetRestaurantById(
                _restaurantService.GetRestaurantIdFromCookieHeader(HttpContext));

            if (restaurant == null)
            {
                return Redirect("/restaurants");
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
