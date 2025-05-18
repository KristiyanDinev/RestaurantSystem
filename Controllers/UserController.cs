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
                return View("Login", loginFormModel);
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user != null) {
                return RedirectToAction("Index", "Restaurant");
            }

            UserModel? loggedIn = await _userService.LoginUser(
                loginFormModel.Email, loginFormModel.Password);

            if (loggedIn == null)
            {
                loginFormModel.Error = "Invalid login attempt.";
                return View("Login", loginFormModel);
            }

            _userUtility.SetUserAuthBearerHeader(
                HttpContext,
                _userUtility.GenerateAuthBearerHeader_JWT(
                    loggedIn, loginFormModel.RememberMe));

            return RedirectToAction("Index", "Restaurant");
        }


        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> RegisterUser(
            [FromForm] RegisterFormModel registerFormModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", registerFormModel);
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user != null)
            {
                return RedirectToAction("Index", "Restaurant");
            }

            UserModel? registered = await _userService.RegisterUser(registerFormModel);
            if (registered == null)
            {
                registerFormModel.Error = "Registration failed. Please try again.";
                return View("Register", registerFormModel);
            }

            _userUtility.SetUserAuthBearerHeader(
                HttpContext,
                _userUtility.GenerateAuthBearerHeader_JWT(
                    registered, registerFormModel.RememberMe));

            return RedirectToAction("Index", "Restaurant");
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
                return RedirectToAction("Profile");
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login");
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

            bool updateSuccessful = await _userService.UpdateUser(user, 
                profileUpdateFormModel);
            user = await _userService.GetUser(user.Id);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            ProfileViewModel profileViewModel = new ProfileViewModel()
            {
                UpdatedSuccessfully = updateSuccessful ? "Profile updated successfully!" : null,
                User = user
            };

            return View("Profile", profileViewModel);
        }


        [HttpPost]
        [Route("/logout")]
        public IActionResult Logout()
        {
            _userUtility.RemoveAuthBearerHeader(HttpContext);

            return RedirectToAction("Login");
        }
    }
}
