using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Identity;
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
        private ILogger<UserController> _logger;

        public UserController(UserService userService,
            UserUtility userUtility,
        EmailService emailService,
        ILogger<UserController> logger)
        {
            _userService = userService;
            _userUtility = userUtility;
            _emailService = emailService;
            _logger = logger;
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
                UserModel? existingUser = await _userService.GetUserByEmailAsync(
                    loginFormModel.Email);
                TempData["LoginError"] = existingUser?.PasswordHash == null
                    ? "This account needs a password reset before it can be used."
                    : "Invalid email or password. Please try again.";
                _logger.LogWarning("Identity login failed for {Email}; account exists: {AccountExists}, has password hash: {HasPasswordHash}",
                    loginFormModel.Email, existingUser != null, existingUser?.PasswordHash != null);
                return BadRequest();
            }

            if (!await _userService.UpdateLastLoginDate(loggedIn))
            {
                TempData["Email"] = loginFormModel.Email;
                TempData["LastLoginUpdateError"] = true;
                return BadRequest();
            }

            TempData["LoginSuccess"] = true;

            await _userUtility.SignInAsync(loggedIn, loginFormModel.RememberMe);

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

            (UserModel? registered, IdentityResult registrationResult) =
                await _userService.RegisterUserAsync(registerFormModel);
            if (registered == null)
            {
                TempData["Email"] = registerFormModel.Email;
                TempData["Name"] = registerFormModel.Name;
                TempData["Error"] = true;
                TempData["IdentityErrors"] = string.Join(
                    " ", registrationResult.Errors.Select(error => error.Description));
                _logger.LogWarning("Identity registration failed for {Email}: {Errors}",
                    registerFormModel.Email,
                    string.Join("; ", registrationResult.Errors.Select(error =>
                        $"{error.Code}: {error.Description}")));
                return BadRequest();
            }

            TempData["RegisterSuccess"] = true;

            await _userUtility.SignInAsync(registered, registerFormModel.RememberMe);

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
        public async Task<IActionResult> Logout()
        {
            await _userUtility.SignOutAsync();
            return Ok();
        }


        [HttpPost]
        [Route("/requestcode")]
        public async Task<IActionResult> RequestCode([FromForm] string Email)
        {
            if (await _userService.GetUserByEmailAsync(Email) == null)
            {
                TempData["RequestCodeError"] = true;
                return BadRequest();
            }
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
            if (model == null || 
            !resetPasswordForm.Code.Equals(model.Code) || 
            DateTime.UtcNow > model.ExpiresAt.ToUniversalTime())
            {
                TempData["VerificationError"] = true;
                return BadRequest();
            }
            await _emailService.DeleteEmailVerificationAsync(resetPasswordForm.Email);
            if (!await _userService.UpdateUserPasswordAsync(resetPasswordForm.Email, resetPasswordForm.NewPassword))
            {
                TempData["ResetError"] = true;
                return BadRequest();
            }
            TempData["ResetSuccess"] = true;
            return Ok();
        }


        [HttpGet]
        [Route("/forgetpassword")]
        public IActionResult ForgetPassword()
        {
            return View();
        }
    }
}
