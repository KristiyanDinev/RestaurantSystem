using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Models.View.User;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers {



    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class UserController : Controller {

        private UserService _userService;
        private UserUtility _userUtility;
        private RoleService _roleService;
        private readonly string deliveryRoleName = "delivery";

        public UserController(UserService userService, UserUtility userUtility,
            RoleService roleService)
        {
            _userService = userService;
            _userUtility = userUtility;
            _roleService = roleService;
        }

        [HttpGet]
        [Route("/")]
        [Route("/login")]
        public async Task<IActionResult> Login()
        {
            return await _userUtility.GetUserByJWT(HttpContext) != null ?
                RedirectToAction("Index", "Restaurant") : View();
        }


        [HttpGet]
        [Route("/register")]
        public async Task<IActionResult> Register()
        {
            return await _userUtility.GetUserByJWT(HttpContext) != null ?
                RedirectToAction("Index", "Restaurant") : View();
        }


        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> LoginUser(
            [FromForm] LoginFormModel loginFormModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Login"] = loginFormModel;
                return BadRequest();
            }

            UserModel? loggedIn = await _userService.LoginUser(
                loginFormModel.Email, loginFormModel.Password);

            if (loggedIn == null)
            {
                TempData["Error"] = "Invalid login attempt.";
                TempData["Login"] = loginFormModel;
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

            UserModel? registered = await _userService.RegisterUser(registerFormModel);
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
            return user == null ? RedirectToAction("Login") :
                View(new ProfileViewModel { User = user });
        }


        [HttpPost]
        [Route("/profile/update")]
        public async Task<IActionResult> ProfileUpdate(
            [FromForm] ProfileUpdateFormModel profileUpdateFormModel)
        {
            // Client wants to update their own profile. Allow all changes.

            // Check if that client is a delivery guy and if it is,
            // then do not update the city, country and state
            // Update these values only by an admin

            // profileUpdateFormModel contains the new/updated information.
            // It also may contain some old information.

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }

            if (!user.City.Equals(profileUpdateFormModel.City) ||
               !user.Country.Equals(profileUpdateFormModel.Country) ||
                user.State != profileUpdateFormModel.State)
            {
                // user changed city, country and state
                List<string> roles = await _roleService.GetUserRoles(user.Id);
                if (roles.Contains(deliveryRoleName)) {
                    // that user is a delivery guy
                    profileUpdateFormModel.City = user.City;
                    profileUpdateFormModel.Country = user.Country;
                    profileUpdateFormModel.State = user.State;
                }
            }

            return await _userService.UpdateUser(user,
                profileUpdateFormModel) ?
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
