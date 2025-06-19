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
        public async Task<IActionResult> Addresses() {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(new AddressesViewModel
            {
                Addresses = await _addressService.GetUserAddressesAsync(user.Id),
                User = user
            });
        }


        [HttpGet]
        [Route("/address/update/{address_id}")]
        public async Task<IActionResult> AddressUpdate(long address_id)
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
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
                User = user
            });
        }


        [HttpGet]
        [Route("/address/add")]
        public async Task<IActionResult> AddressAdd() {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(user);
        }

        [HttpPost]
        [Route("/address/{address_id}")]
        public async Task<IActionResult> DeleteAddress(long address_id)
        {
            UserModel? user = await _userUtility.GetUserByJWT(HttpContext);
            if (user == null)
            {
                return Unauthorized();
            }

            if (await _addressService.DeleteAddressAsync(address_id))
            {
                TempData["Success"] = "Address deleted successfully.";
                return Ok();
            }
            else
            {
                return BadRequest();
            }
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
                TempData["Success"] = "Address updated successfully.";
                return Ok();
            }
            else 
            {
                return BadRequest();
            }
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
                TempData["Success"] = "Address added successfully.";
                return Ok();

            } else
            {
                return BadRequest();
            }
        }
    }
}
