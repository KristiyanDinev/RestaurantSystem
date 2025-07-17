using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Services;

namespace RestaurantSystem.Controllers
{

    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class CuponCodeController : Controller
    {
        private CuponService _cuponService;
        public CuponCodeController(CuponService cuponService)
        {
            _cuponService = cuponService;
        }


        [HttpPost]
        [Route("/cupon/validate")]
        public async Task<IActionResult> ValidateCuponCode(
            [FromForm] string CuponCode,
            [FromForm] decimal Total)
        {
            if (string.IsNullOrEmpty(CuponCode))
            {
                TempData["CuponInvalid"] = true;
                return BadRequest();
            }
            TempData["CuponCode"] = CuponCode;
            CuponModel? code = await _cuponService.GetCuponByCodeAsync(CuponCode);
            if (code == null)
            {
                TempData["CuponInvalid"] = true;
                return NotFound();
            }
            if (code.ExpirationDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                TempData["CuponInvalid"] = true;
                return BadRequest();
            }
            TempData["CuponValid"] = true;
            TempData["CuponDiscount"] = code.DiscountPercent;
            TempData["CuponTotal"] = _cuponService.HandleCuponDiscount(code.DiscountPercent, Total).ToString();
            return Ok();
        }
    }
}
