using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Models.View.Address;
using RestaurantSystem.Services;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Controllers
{
    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class AddressController : Controller
    {
        private UserUtility _userUtility;
        private AddressService _addressService;

        public AddressController(UserUtility userUtility,
            AddressService addressService) {
            _userUtility = userUtility;
            _addressService = addressService;
        }


        [HttpGet]
        [Route("/addresses")]
        public async Task<IActionResult> Addresses([FromQuery] int page = 1) {
            UserModel? user = await _userUtility.GetUserWithRolesByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(new AddressesViewModel
            {
                Addresses = await _addressService.GetUserAddressesAsync(user.Id, page),
                User = user,
                Page = page,
            });
        }


        [HttpGet]
        [Route("/address/update/{address_id}")]
        public async Task<IActionResult> AddressUpdate(long address_id)
        {
            UserModel? user = await _userUtility.GetUserWithRolesByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            AddressModel? address = await _addressService.GetAddressByIdAsync(address_id);
            if (address == null)
            {
                return NotFound();
            }

            return View(new AddressesViewModel
            {
                Addresses = new List<AddressModel> { address },
                User = user,
                Page = -1, // -1 indicates this is an update page
            });
        }


        [HttpGet]
        [Route("/address/add")]
        public async Task<IActionResult> AddressAdd() {
            UserModel? user = await _userUtility.GetUserWithRolesByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(user);
        }


        [HttpPost]
        [Route("/address/delete/{address_id}")]
        public async Task<IActionResult> DeleteAddress(long address_id)
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return Unauthorized();
            }

            if (await _addressService.DeleteAddressAsync(address_id))
            {
                TempData["DeleteSuccess"] = true;
                return Ok();
            }

            TempData["DeleteError"] = true;
            return BadRequest();
        }


        [HttpPost]
        [Route("/address/update")]
        public async Task<IActionResult> UpdateAddress(
            [FromForm] AddressUpdateFormModel addressUpdateForm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return Unauthorized();
            }

            if (await _addressService.UpdateAddressAsync(addressUpdateForm))
            {
                TempData["UpdateSuccess"] = true;
                return Ok();
            }
            TempData["UpdateError"] = true;
            return BadRequest();
        }


        [HttpPost]
        [Route("/address/add")]
        public async Task<IActionResult> AddAddress(
            [FromForm] AddAddressFormModel addAddressForm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return Unauthorized();
            }

            if (await _addressService.AddAddressAsync(user.Id, addAddressForm))
            {
                TempData["AddSuccess"] = true;
                return Ok();

            } 
            TempData["AddError"] = true;
            return BadRequest();
        }
    }
}
