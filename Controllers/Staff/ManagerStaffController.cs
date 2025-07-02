using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Enums;
using RestaurantSystem.Models.DatabaseModels;
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
        private RestaurantService _restaurantService;
        private RoleService _roleService;
        private UserService _userService;

        public ManagerStaffController(UserUtility userUtils,
            RestaurantService restaurantService,
            RoleService roleService,
            UserService userService)
        {
            _userUtils = userUtils;
            _restaurantService = restaurantService;
            _roleService = roleService;
            _userService = userService;
        }


        [HttpGet]
        [Route("/staff/manager")]
        public async Task<IActionResult> Manager()
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "Staff");
            }
            return View(user);
        }


        [HttpGet]
        [Route("/staff/manager/employees")]
        public async Task<IActionResult> Employees([FromQuery] int page = 1)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "Staff");
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
                [FromForm] long Id, [FromForm] RoleEnum Role)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext);
            if (user == null || user.Restaurant == null)
            {
                return BadRequest();
            }
            UserModel? employee = await _userService.GetUserAsync(Id);
            if (employee == null) {
                return BadRequest();
            }
            if (await _roleService.AssignRoleToUserAsync(Id, Role))
            {
                TempData["GiveRole"] = true;
                return Ok();
            }
            return BadRequest();
        }


        [HttpPost]
        [Route("/staff/manager/employees/role/remove")]
        public async Task<IActionResult> RemoveEmployeeRole(
                [FromForm] long Id, [FromForm] RoleEnum Role)
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
            if (await _roleService.RemoveRoleFromUserAsync(Id, Role))
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
                return RedirectToAction("Login", "Staff");
            }
            return View();
        }


        [HttpGet]
        [Route("/staff/manager/deliveries")]
        public async Task<IActionResult> Deliveries([FromQuery] int page = 1)
        {
            UserModel? user = await _userUtils.GetStaffUserByJWT(HttpContext, true);
            if (user == null || user.Restaurant == null)
            {
                return RedirectToAction("Login", "Staff");
            }
            return View();
        }
    }
}
