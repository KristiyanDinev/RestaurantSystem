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
            DeliveryModel? delivery = await _deliveryService.GetDeliveryAsync(user.Id);
            if (delivery != null)
            {
                return RedirectToAction("DeliveryMyOwner");
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
        public async Task<IActionResult> DeliveryRestaurant()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }
            DeliveryModel? delivery = await _deliveryService.GetDeliveryAsync(user.Id);
            if (delivery != null)
            {
                return RedirectToAction("DeliveryMyOwner");
            }
            AddressModel? address = await _deliveryService.GetDeliveryAddressCookie(HttpContext);
            if (address == null)
            {
                return RedirectToAction("DeliveryAddress");
            }

            return View(new DeliveryRestaurantViewModel()
            {
                Staff = user,
                Restaurants = await _restaurantService.GetDeliveryGuy_RestaurantsAsync(address)
            });
        }


        [HttpGet]
        [Route("/staff/delivery/orders")]
        public async Task<IActionResult> DeliveryOrders()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }
            DeliveryModel? delivery = await _deliveryService.GetDeliveryAsync(user.Id);
            if (delivery != null)
            {
                return RedirectToAction("DeliveryMyOwner");
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
                Orders = orders,
                Restaurant = restaurant
            });
        }


        [HttpGet]
        [Route("/staff/delivery/myorder")]
        public async Task<IActionResult> DeliveryMyOwner()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            DeliveryModel? delivery = await _deliveryService.GetDeliveryAsync(user.Id);
            if (delivery == null)
            {
                return RedirectToAction("DeliveryOrders");
            }

            return View(new DeliveryMyOrderViewModel()
            {
                Order = new OrderWithDishesCountModel()
                {
                    Order = delivery.Order,
                    DishesCount = await _orderedDishesService
                        .CountDishesByOrderAsync(delivery.Order.Id)
                },
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

            DeliveryModel? delivery = await _deliveryService.GetDeliveryAsync(user.Id);
            if (delivery != null)
            {
                return BadRequest();
            }

            if (await _deliveryService.AddDeliveryAsync(user.Id, orderId) &&
                await _orderService.UpdateOrderCurrentStatusByIdAsync(orderId, 
                                            OrderStatusEnum.Delivering)) {
                TempData["Success"] = true;
                return Ok();
            }

            return BadRequest();
        }


        [HttpPost]
        [Route("/staff/delivery/delivered")]
        public async Task<IActionResult> MarkAsDelivered()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }
            DeliveryModel? delivery = await _deliveryService.GetDeliveryAsync(user.Id);
            if (delivery == null)
            {
                return BadRequest();
            }
            if (await _orderService.UpdateOrderCurrentStatusByIdAsync(delivery.OrderId,
                                            OrderStatusEnum.Delivered))
            {
                TempData["DeliveredSuccessfully"] = true;
                return Ok();
            }
            return Ok();
        }


        [HttpPost]
        [Route("/staff/delivery/cancel")]
        public async Task<IActionResult> CancelDelivery()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null)
            {
                return BadRequest();
            }

            DeliveryModel? delivery = await _deliveryService.GetDeliveryAsync(user.Id);
            if (delivery == null)
            {
                return BadRequest();
            }
            if (await _deliveryService.RemoveDeliveryAsync(user.Id) &&
                await _orderService.UpdateOrderCurrentStatusByIdAsync(delivery.OrderId, 
                                            OrderStatusEnum.Ready)) {
                TempData["Canceled"] = true;
                return Ok();
            }
            return BadRequest();
        }
    }
}
