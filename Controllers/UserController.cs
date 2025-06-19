using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers {



    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class UserController : Controller {

        private UserService _userService;
        private UserUtility _userUtility;

        public UserController(UserService userService, UserUtility userUtility)
        {
            _userService = userService;
            _userUtility = userUtility;
        }

        [HttpGet]
        [Route("/")]
        [Route("/login")]
        public async Task<IActionResult> Login()
        {
            return await _userUtility.GetUserByJWT(HttpContext) != null ?
                RedirectToAction("Addresses", "Address") : View();
        }


        [HttpGet]
        [Route("/register")]
        public async Task<IActionResult> Register()
        {
            return await _userUtility.GetUserByJWT(HttpContext) != null ?
                RedirectToAction("Addresses", "Address") : View();
        }


        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> LoginUser(
            [FromForm] LoginFormModel loginFormModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserModel? loggedIn = await _userService.LoginUserAsync(
                loginFormModel.Email, loginFormModel.Password);

            if (loggedIn == null)
            {
                TempData["Error"] = "Invalid login attempt.";
                return BadRequest();
            }

            _userUtility.SetUserAuthBearerHeader(
                HttpContext,
                _userUtility.GenerateAuthBearerHeader_JWT(
                    loggedIn, loginFormModel.RememberMe));

            return Ok();
        }


        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> RegisterUser(
            [FromForm] RegisterFormModel registerFormModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserModel? registered = await _userService.RegisterUserAsync(registerFormModel);
            if (registered == null)
            {
                TempData["Error"] = "Invalid registration attempt.";
                return BadRequest();
            }

            _userUtility.SetUserAuthBearerHeader(
                HttpContext,
                _userUtility.GenerateAuthBearerHeader_JWT(
                    registered, registerFormModel.RememberMe));

            return Ok();
        }



        [HttpGet]
        [Route("/profile")]
        public async Task<IActionResult> Profile()
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            return user == null ? RedirectToAction("Login") : View(user);
        }


        [HttpPost]
        [Route("/profile/update")]
        public async Task<IActionResult> ProfileUpdate(
            [FromForm] ProfileUpdateFormModel profileUpdateFormModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }

            return await _userService.UpdateUserAsync(user, profileUpdateFormModel) ?
                Ok() : BadRequest();
        }


        [HttpPost]
        [Route("/logout")]
        public IActionResult Logout()
        {
            _userUtility.RemoveAuthBearerHeader(HttpContext);
            return Ok();
        }
    }
}
