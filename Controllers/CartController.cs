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

        public CartController(DishService dishService, UserUtility userUtility)
        {
            _dishService = dishService;
            _userUtility = userUtility;
        }


        [HttpGet]
        [Route("/Cart")]
        public async Task<IActionResult> Cart()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            CartViewModel cartViewModel = new CartViewModel();

            cartViewModel.User = user;
            cartViewModel.Dishes = await _dishService.GetDishesByIds(
                _dishService.GetDishIDsFromCart(HttpContext));

            return View(cartViewModel);
        }
    }
}
