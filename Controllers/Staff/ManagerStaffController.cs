﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Enums;
using RestaurantSystem.Models;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Models.View.Staff.Manager;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers.Staff
{

    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class ManagerStaffController : Controller
    {
        private UserUtility _userUtils;
        private RoleService _roleService;
        private UserService _userService;
        private DishService _dishService;
        private OrderService _orderService;
        private OrderedDishesService _orderedDishesService;
        private DeliveryService _deliveryService;

        public ManagerStaffController(UserUtility userUtils,
            RoleService roleService,
            UserService userService,
            DishService dishService,
            OrderService orderService,
            OrderedDishesService orderedDishesService,
            DeliveryService deliveryService)
        {
            _userUtils = userUtils;
            _roleService = roleService;
            _userService = userService;
            _dishService = dishService;
            _orderService = orderService;
            _orderedDishesService = orderedDishesService;
            _deliveryService = deliveryService;
        }


        [HttpGet]
        [Route("/staff/manager/employees")]
        public async Task<IActionResult> Employees([FromQuery] int page = 1)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(new ManagerEmployeesViewModel() { 
                Page = page,
                Staff = user,
                Employees = await _userService
                    .GetRestaurantEmployeesAsync(user.Restaurant.Id, page)
            });
        }


        [HttpPost]
        [Route("/staff/manager/employees/add")]
        public async Task<IActionResult> AddEmployee([FromForm] string Email)
        {
            if (string.IsNullOrWhiteSpace(Email)) {
                return BadRequest();
            }
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return BadRequest();
            }
            UserModel? newEmployee = await _userService.GetUserByEmailAsync(Email);
            if (newEmployee == null)
            {
                return BadRequest();
            }
            if (await _userService.AddEmployeeToRestaurantAsync(newEmployee, user.Restaurant.Id)) {
                TempData["AddStaff"] = true;
                return Ok();
            }
            return BadRequest();
        }


        [HttpPost]
        [Route("/staff/manager/employees/remove")]
        public async Task<IActionResult> RemoveEmployee([FromForm] long Id)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return BadRequest();
            }
            UserModel? employee = await _userService.GetUserAsync(Id);
            if (employee == null)
            {
                return BadRequest();
            }
            if (await _userService.RemoveEmployeeFromRestaurantAsync(employee))
            {
                TempData["RemoveStaff"] = true;
                return Ok();
            }
            return BadRequest();
        }


        [HttpPost]
        [Route("/staff/manager/employees/role/give")]
        public async Task<IActionResult> GiveEmployeeRole(
                [FromForm] EmployeeRoleFormModel employeeRoleFormModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return BadRequest();
            }
            UserModel? employee = await _userService.GetUserAsync(employeeRoleFormModel.Id);
            if (employee == null) {
                return BadRequest();
            }
            if (await _roleService.AssignRoleToUserAsync(
                employeeRoleFormModel.Id, employeeRoleFormModel.Role))
            {
                TempData["GiveRole"] = true;
                return Ok();
            }
            return BadRequest();
        }


        [HttpPost]
        [Route("/staff/manager/employees/role/remove")]
        public async Task<IActionResult> RemoveEmployeeRole(
                [FromForm] EmployeeRoleFormModel employeeRoleFormModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return BadRequest();
            }
            UserModel? employee = await _userService.GetUserAsync(employeeRoleFormModel.Id);
            if (employee == null)
            {
                return BadRequest();
            }
            if (await _roleService.RemoveRoleFromUserAsync(
                employeeRoleFormModel.Id, employeeRoleFormModel.Role))
            {
                TempData["RemoveRole"] = true;
                return Ok();
            }
            return BadRequest();
        }


        [HttpGet]
        [Route("/staff/manager/dishes")]
        public async Task<IActionResult> Dishes([FromQuery] int page = 1)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(new ManagerDishViewModel()
            {
                Dishes = await _dishService
                    .GetDishesByRestaurantIdAsync(user.Restaurant.Id, page),
                Staff = user,
                Page = page
            });
        }


        [HttpPost]
        [Route("/staff/manager/dishes/create")]
        public async Task<IActionResult> CreateDishes([FromForm] CreateDishFormModel createDishFormModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return BadRequest();
            }
            if (await _dishService.CreateDishAsync(createDishFormModel, user.Restaurant.Id)) {
                TempData["CreatedSuccessfully"] = true;
                return Ok();
            }
            return BadRequest();
        }


        [HttpPost]
        [Route("/staff/manager/dishes/edit")]
        public async Task<IActionResult> EditDishes([FromForm] EditDishFormModel editDishFormModel)
        {
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return BadRequest();
            }
            if (await _dishService.UpdateDishAsync(editDishFormModel))
            {
                TempData["UpdatedSuccessfully"] = true;
                return Ok();
            }
            return BadRequest();
        }


        [HttpPost]
        [Route("/staff/manager/dishes/delete/{dishId}")]
        public async Task<IActionResult> DeleteDishes(int dishId)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return BadRequest();
            }
            if (await _dishService.DeleteDishAsync(dishId))
            {
                TempData["DeletedSuccessfully"] = true;
                return Ok();
            }
            return BadRequest();
        }


        [HttpGet]
        [Route("/staff/manager/deliveries")]
        public async Task<IActionResult> Deliveries([FromQuery] int page = 1)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "User");
            }

            List<OrderWithDishesCountModel> dishes = new();
            foreach (OrderModel order in await _orderService
                    .GetDeliveredOrdersAsync(user.Restaurant.Id, page))
            {
                dishes.Add(new OrderWithDishesCountModel()
                {
                    Order = order,
                    DishesCount = await _orderedDishesService
                        .CountDishesByOrderAsync(order.Id)
                });
            }
            return View(new ManagerDeliveryViewModel()
            {
                Staff = user,
                Orders = dishes,
                Page = page
            });
        }


        [HttpPost]
        [Route("/staff/manager/deliveries/cancel/{orderId}")]
        public async Task<IActionResult> CancelDelivery(long orderId)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
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
                                            OrderStatusEnum.Ready))
            {
                TempData["Canceled"] = true;
                return Ok();
            }
            return BadRequest();
        }


        [HttpPost]
        [Route("/staff/manager/deliveries/delete/{orderId}")]
        public async Task<IActionResult> DeleteDeliveryAndOrder(long orderId)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return BadRequest();
            }
            DeliveryModel? delivery = await _deliveryService.GetDeliveryAsync(user.Id);
            if (delivery == null)
            {
                return BadRequest();
            }
            if (await _deliveryService.RemoveDeliveryAsync(user.Id) &&
                await _orderService.DeleteOrderAsync(delivery.OrderId, false))
            {
                TempData["Deleted"] = true;
                return Ok();
            }
            return BadRequest();
        }
    }
}
