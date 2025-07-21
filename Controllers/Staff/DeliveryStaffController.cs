using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Enums;
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
        public async Task<IActionResult> DeliveryAddress([FromQuery] int page = 1)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View(new AddressesViewModel()
            {
                User = user,
                Addresses = await _addressService.GetUserAddressesAsync(user.Id, page),
                Page = page,
            });
        }


        [HttpGet]
        [Route("/staff/delivery/restaurant")]
        public async Task<IActionResult> DeliveryRestaurant([FromQuery] int page = 1)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
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
                Staff = user,
                Page = page,
                Restaurants = await _restaurantService.GetDeliveryGuy_RestaurantsAsync(address, page)
            });
        }


        [HttpGet]
        [Route("/staff/delivery/orders")]
        public async Task<IActionResult> DeliveryOrders([FromQuery] int page = 1)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }
            RestaurantModel? restaurant = await _deliveryService.GetDeliveryRestaurantCookie(HttpContext);
            if (restaurant == null)
            {
                return RedirectToAction("DeliveryRestaurant");
            }
            List<OrderWithDishesCountModel> orders = new ();
            foreach (OrderModel order in 
                await _orderService.GetDeliveryOrdersByRestaurantIdAsync(restaurant.Id, page))
            {
                orders.Add(new OrderWithDishesCountModel()
                {
                    Order = order,
                    DishesCount = await _orderedDishesService.CountDishesByOrderAsync(order.Id)
                });
            }
            user.Deliveries = await _deliveryService.GetDeliveriesAsync(user.Id);
            return View(new DeliveryOrdersViewModel()
            {
                Staff = user,
                Orders = orders,
                Page = page,
                Restaurant = restaurant
            });
        }


        [HttpGet]
        [Route("/staff/delivery/myorders")]
        public async Task<IActionResult> DeliveryMyOrder()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            List<DeliveryModel> deliveries = await _deliveryService.GetDeliveriesAsync(user.Id);
            if (deliveries.Count == 0)
            {
                return RedirectToAction("DeliveryOrders");
            }

            List<OrderWithDishesCountModel> orders = new();
            foreach (DeliveryModel deliveryModel in deliveries)
            { 
                orders.Add(new OrderWithDishesCountModel()
                {
                    Order = deliveryModel.Order,
                    DishesCount = await _orderedDishesService
                        .CountDishesByOrderAsync(deliveryModel.Order.Id)
                });
            }

            return View(new DeliveryMyOrderViewModel()
            {
                Orders = orders,
                Staff = user
            });
        }


        [HttpPost]
        [Route("/staff/delivery/start/{orderId}")]
        public async Task<IActionResult> StartDelivery(long orderId)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }
            
            if (await _deliveryService.AddDeliveryAsync(user.Id, orderId) &&
                await _orderService.UpdateOrderCurrentStatusByIdAsync(orderId,
                                            OrderStatusEnum.Delivering))
            {
                TempData["Success"] = true;
                return Ok();
            }
            TempData["Error"] = true;
            return BadRequest();
        }


        [HttpPost]
        [Route("/staff/delivery/delivered/{orderId}")]
        public async Task<IActionResult> MarkAsDelivered(long orderId)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }
            List<DeliveryModel> deliveries = await _deliveryService.GetDeliveriesAsync(user.Id);
            if (deliveries.Count == 0)
            {
                return BadRequest();
            }
            if (deliveries.Where(d => d.OrderId == orderId).FirstOrDefault() != null &&
                await _orderService.UpdateOrderCurrentStatusByIdAsync(orderId,
                                            OrderStatusEnum.Delivered))
            {
                TempData["DeliveredSuccessfully"] = true;
                return Ok();
            }
            TempData["DeliveredError"] = true;
            return BadRequest();
        }


        [HttpPost]
        [Route("/staff/delivery/cancel/{orderId}")]
        public async Task<IActionResult> CancelDelivery(long orderId)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }
            List<DeliveryModel> deliveries = await _deliveryService.GetDeliveriesAsync(user.Id);
            if (deliveries.Count == 0)
            {
                return BadRequest();
            }
            if (deliveries.Where(d => d.OrderId == orderId).FirstOrDefault() != null &&
                await _deliveryService.RemoveDeliveryAsync(user.Id, orderId) &&
                await _orderService.UpdateOrderCurrentStatusByIdAsync(orderId, 
                                            OrderStatusEnum.Ready)) {
                TempData["CanceledSuccess"] = true;
                return Ok();
            }
            TempData["CanceledError"] = true;
            return BadRequest();
        }
    }
}
