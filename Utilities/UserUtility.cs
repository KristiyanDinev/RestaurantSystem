using Microsoft.AspNetCore.Identity;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Services;

namespace RestaurantSystem.Utilities
{
    public class UserUtility
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserService _userService;

        public UserUtility(UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager, UserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        public Task SignInAsync(UserModel user, bool rememberMe) =>
            _signInManager.SignInAsync(user, rememberMe);

        public Task SignOutAsync() => _signInManager.SignOutAsync();

        public Task<UserModel?> GetUserByJWT(HttpContext context) =>
            _userManager.GetUserAsync(context.User);

        public async Task<UserModel?> GetStaffUserByJWT(HttpContext context,
            bool getRoles = false)
        {
            UserModel? user = await GetUserByJWT(context);
            if (user == null)
            {
                return null;
            }

            UserModel? staffUser = await _userService.GetStaffUserAsync(user.Id);
            return getRoles && staffUser != null
                ? await _userService.GetStaffUserWithRolesAsync(user.Id)
                : staffUser;
        }

        public async Task<UserModel?> GetUserWithRolesByJWT(HttpContext context)
        {
            UserModel? user = await GetUserByJWT(context);
            return user == null ? null : await _userService.GetUserWithRolesAsync(user.Id);
        }

        public void RemoveCartCookie(HttpContext context) =>
            context.Response.Cookies.Delete("cart_items");

        public void RemoveDeliveryAddressCookie(HttpContext context) =>
            context.Response.Cookies.Delete("delivery_address_id");

        public void RemoveDeliveryRestaurantCookie(HttpContext context) =>
            context.Response.Cookies.Delete("delivery_restaurant_id");
    }
}
