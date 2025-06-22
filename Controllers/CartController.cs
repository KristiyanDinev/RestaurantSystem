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
    [IgnoreAntiforgeryToken]
    public class CartController : Controller
    {
        private UserUtility _userUtility;
        private DishService _dishService;
        private RestaurantService _restaurantService;
        private AddressService _addressService;

        public CartController(DishService dishService, UserUtility userUtility, 
            RestaurantService restaurantService, AddressService addressService)
        {
            _dishService = dishService;
            _userUtility = userUtility;
            _restaurantService = restaurantService;
            _addressService = addressService;
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

            RestaurantModel? restaurant = await _restaurantService.GetRestaurantByIdAsync(
                _restaurantService.GetRestaurantIdFromCookieHeaderAsync(HttpContext));

            if (restaurant == null)
            {
                return RedirectToAction("Index", "Restaurant");
            }


            List<int> ids = _dishService.GetDishIDsFromCartAsync(HttpContext);
            HashSet<int> hash_ids = new(ids);
            Dictionary<DishModel, int> dishes = new();
            List<DishModel> dishModels = await _dishService.GetDishesByIdsAsync(hash_ids);

            foreach (int eachDishId in hash_ids)
            {
                int countOfItems = ids.Count;
                ids.RemoveAll(id => id == eachDishId);

                DishModel? dish = dishModels.FirstOrDefault(dish => dish.Id == eachDishId);

                if (dish != null)
                {
                    dishes.Add(dish, countOfItems - ids.Count);
                }
            }

            return View(new CartViewModel()
            {
                User = user,
                Restaurant = restaurant,
                Dishes = dishes,
                Addresses = await _addressService.GetUserAddressesAsync(user.Id)
            });
        }
    }
}
