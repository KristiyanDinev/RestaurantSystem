using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Services;

namespace RestaurantSystem.Controllers
{

    [ApiController]
    [EnableRateLimiting("fixed")]
    [IgnoreAntiforgeryToken]
    public class CouponCodeController : Controller
    {
        private CouponService _couponService;
        public CouponCodeController(CouponService couponService)
        {
            _couponService = couponService;
        }


        [HttpPost]
        [Route("/coupon/validate")]
        public async Task<IActionResult> ValidateCouponCode(
            [FromForm] string CouponCode,
            [FromForm] decimal Total)
        {
            if (string.IsNullOrEmpty(CouponCode))
            {
                return BadRequest();
            }
            CouponModel? code = await _couponService.GetCouponByCodeAsync(CouponCode);
            if (code == null)
            {
                return NotFound();
            }
            if (code.ExpirationDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                return BadRequest();
            }
            TempData["CouponDiscount"] = code.DiscountPercent;
            TempData["CouponTotal"] = _couponService.HandleCouponDiscount(code.DiscountPercent, Total).ToString();
            return Ok();
        }
    }
}
