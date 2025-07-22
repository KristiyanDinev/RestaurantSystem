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
    public class UserController : Controller
    {

        private UserService _userService;
        private UserUtility _userUtility;
        private EmailService _emailService;

        public UserController(UserService userService,
            UserUtility userUtility,
            EmailService emailService)
        {
            _userService = userService;
            _userUtility = userUtility;
            _emailService = emailService;
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
                TempData["Email"] = loginFormModel.Email;
                TempData["Error"] = true;
                return BadRequest();
            }

            if (!await _userService.UpdateLastLoginDate(loggedIn))
            {
                TempData["Email"] = loginFormModel.Email;
                TempData["LastLoginUpdateError"] = true;
                return BadRequest();
            }

            TempData["LoginSuccess"] = true;

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
                TempData["Email"] = registerFormModel.Email;
                TempData["Name"] = registerFormModel.Name;
                TempData["Error"] = true;
                return BadRequest();
            }

            TempData["RegisterSuccess"] = true;

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
            UserModel? user = await _userUtility.GetUserWithRolesByJWT(HttpContext);
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
            if (await _userService.UpdateUserAsync(user, profileUpdateFormModel))
            {
                TempData["ProfileUpdateSuccess"] = true;
                return Ok();
            }
            TempData["ProfileUpdateError"] = true;
            return BadRequest();
        }


        [HttpPost]
        [Route("/logout")]
        public IActionResult Logout()
        {
            _userUtility.RemoveAuthBearerHeader(HttpContext);
            return Ok();
        }


        [HttpPost]
        [Route("/requestcode")]
        public async Task<IActionResult> RequestCode([FromForm] string Email)
        {
            EmailVerificationModel? model = await _emailService.GetEmailVerificationAsync(Email);
            if (model != null)
            {
                TempData["ExpTime"] = model.ExpiresAt;
                return BadRequest();
            }
            if (!await _emailService.AddEmailCodeAsync(Email))
            {
                TempData["RequestCodeError"] = true;
                return BadRequest();
            }
            TempData["RequestCodeSuccess"] = true;
            return Ok();
        }


        [HttpPost]
        [Route("/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordFormModel resetPasswordForm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            EmailVerificationModel? model = await _emailService.GetEmailVerificationAsync(resetPasswordForm.Email);
            if (model == null || !resetPasswordForm.Code.Equals(model.Code) || DateTime.UtcNow > model.ExpiresAt)
            {
                TempData["VerificationError"] = true;
                return BadRequest();
            }
            if (!await _userService.UpdateUserPasswordAsync(resetPasswordForm.Email, resetPasswordForm.NewPassword))
            {
                TempData["ResetError"] = true;
                return BadRequest();
            }
            TempData["ResetSuccess"] = true;
            return Ok();
        }
    }
}
