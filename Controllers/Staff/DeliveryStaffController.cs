using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.View.Address;
using RestaurantSystem.Models.View.Staff.Delivery;
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
        private AddressService _addressService;
        private DeliveryService _deliveryService;
        private OrderedDishesService _orderedDishesService;

        public DeliveryStaffController(UserUtility userUtils,
            OrderService orderService,
            RestaurantService restaurantService, 
            AddressService addressService,
            DeliveryService deliveryService,
            OrderedDishesService orderedDishesService)
        {
            _userUtils = userUtils;
            _orderService = orderService;
            _restaurantService = restaurantService;
            _addressService = addressService;
            _deliveryService = deliveryService;
            _orderedDishesService = orderedDishesService;
        }

        [HttpGet]
        [Route("/staff/delivery/address")]
        public async Task<IActionResult> DeliveryAddress()
        {
            UserModel? user = await _userUtils.GetUserWithRolesByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(new AddressesViewModel()
            {
                User = user,
                Addresses = await _addressService.GetUserAddressesAsync(user.Id)
            });
        }


        [HttpGet]
        [Route("/staff/delivery/restaurant")]
        public async Task<IActionResult> DeliveryRestaurant()
        {
            UserModel? user = await _userUtils.GetUserWithRolesByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            AddressModel? address = await _deliveryService.GetDeliveryAddressCookie(HttpContext);
            if (address == null)
            {
                return RedirectToAction("DeliveryAddress");
            }

            return View(new DeliveryRestaurantViewModel()
            {
                User = user,
                Restaurants = await _restaurantService.GetDeliveryGuy_RestaurantsAsync(address)
            });
        }


        [HttpGet]
        [Route("/staff/delivery/orders")]
        public async Task<IActionResult> DeliveryOrders()
        {
            UserModel? user = await _userUtils.GetUserWithRolesByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            AddressModel? address = await _deliveryService.GetDeliveryAddressCookie(HttpContext);
            if (address == null)
            {
                return RedirectToAction("DeliveryAddress");
            }

            RestaurantModel? restaurant = await _deliveryService.GetDeliveryRestaurantCookie(HttpContext);
            if (restaurant == null)
            {
                return RedirectToAction("DeliveryRestaurant");
            }

            List<OrderWithDishesCountModel> orders = new ();
            foreach (OrderModel order in 
                await _orderService.GetDeliveryOrdersByRestaurantIdAsync(restaurant.Id))
            {
                orders.Add(new OrderWithDishesCountModel()
                {
                    Order = order,
                    DishesCount = await _orderedDishesService.CountDishesByOrderAsync(order.Id)
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
