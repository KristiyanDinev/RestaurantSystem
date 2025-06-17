using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models;
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
            // Delivery people can take orders from different restaurants in their own city.
            // They just have to update their profile in order to change which restaurants they can take 
            // orders and deliver them
        }

        [HttpGet]
        [Route("/staff/delivery")]
        public async Task<IActionResult> Delivery()
        {
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }


            return View();
            /*
            return View(new DeliveryOrderViewModel()
            {
                Staff = user,
                Order = 
            });*/
        }


        [HttpGet]
        [Route("/staff/delivery/orders")]
        public async Task<IActionResult> DeliveryOrders()
        {
            UserModel? user = await _userUtils.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            List<RestaurantWithOrdersModel> orders = new();

            foreach (RestaurantModel restaurant in await _restaurantService
                .GetDeliveryGuy_RestaurantsAsync("Bulgaria", null, null, user))
            {

                orders.Add(new RestaurantWithOrdersModel()
                {
                    Restaurant = restaurant,
                    Orders = await _orderService
                        .Get_HomeDelivery_OrdersBy_RestaurantIdAsync(restaurant.Id)
                });
            }

            return View(new DeliveryOrdersViewModel()
            {
                Staff = user,
                Orders = orders
            });
        }


        [HttpPost]
        [Route("/staff/delivery")]
        public async Task<IActionResult> UpdateDelivery()
        {
            return Ok();
        }
    }
}
