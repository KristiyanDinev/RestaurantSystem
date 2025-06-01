using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.View.Staff;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers.Staff
{

    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class DeliveryStaffController : Controller
    {
        private OrderService _orderService;
        private UserUtility _userUtils;
        private RestaurantService _restaurantService;

        public DeliveryStaffController(UserUtility userUtils,
            OrderService orderService,
            RestaurantService restaurantService)
        {
            _userUtils = userUtils;
            _orderService = orderService;
            _restaurantService = restaurantService;
        }


        [HttpGet]
        [Route("/staff/delivery")]
        public async Task<IActionResult> Delivery()
        {
            // Delivery people can take orders from different restaurants in their own city.
            // They just have to update their profile in order to change which restaurants they can take 
            // orders and deliver them
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            Dictionary<RestaurantModel, List<OrderModel>> orders = new();

            foreach (RestaurantModel restaurant in await _restaurantService
                .GetDeliveryGuy_Restaurants(user))
            {

                orders.Add(restaurant, await _orderService
                    .Get_HomeDelivery_OrdersBy_RestaurantId(restaurant.Id));
            }

            return View(new DeliveryViewModel()
            {
                Staff = user,
                Orders = orders
            });
        }
    }
}
