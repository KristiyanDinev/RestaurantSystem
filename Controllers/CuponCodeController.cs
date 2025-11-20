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
                TempData["CouponInvalid"] = true;
                return BadRequest();
            }
            TempData["CouponCode"] = CouponCode;
            CouponModel? code = await _couponService.GetCouponByCodeAsync(CouponCode);
            if (code == null)
            {
                TempData["CouponInvalid"] = true;
                return NotFound();
            }
            if (code.ExpirationDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                TempData["CouponInvalid"] = true;
                return BadRequest();
            }
            TempData["CouponValid"] = true;
            TempData["CouponDiscount"] = code.DiscountPercent;
            TempData["CouponTotal"] = _couponService.HandleCouponDiscount(code.DiscountPercent, Total).ToString();
            return Ok();
        }
    }
}
