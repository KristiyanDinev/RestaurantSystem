using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Services
{
    public class CouponService
    {
        private DatabaseContext _databaseContext;

        public CouponService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }


        public async Task<bool> DeleteCouponAsync(string couponCode)
        {
            CouponModel? coupon = await GetCouponByCodeAsync(couponCode);
            if (coupon == null)
            {
                return false;
            }
            _databaseContext.Coupons.Remove(coupon);
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<List<CouponModel>> GetCouponsAsync(int page)
        {
            return await Utility.GetPageAsync<CouponModel>(
                _databaseContext.Coupons
                .OrderByDescending(c => c.DiscountPercent)
                .AsQueryable(), page
            ).ToListAsync();
        }

        public async Task<CouponModel?> GetCouponByCodeAsync(string couponCode)
        {
            return await _databaseContext.Coupons.FirstOrDefaultAsync(
                c => c.CouponCode.Equals(couponCode));
        }

        public async Task<bool> EditCouponAsync(CouponCreateFormModel model)
        {
            CouponModel? coupon = await GetCouponByCodeAsync(model.CouponCode);
            if (coupon == null)
            {
                return false;
            }
            coupon.Name = model.Name;
            coupon.DiscountPercent = model.DiscountPercent;
            coupon.ExpirationDate = model.ExpDate;
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateCouponAsync(CouponCreateFormModel model)
        {
            await _databaseContext.Coupons.AddAsync(new CouponModel()
            {
                Name = model.Name,
                CouponCode = model.CouponCode,
                DiscountPercent = model.DiscountPercent,
                ExpirationDate = model.ExpDate
            });
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public decimal HandleCouponDiscount(int discountPercent, decimal totalPrice)
        {
            return totalPrice - (totalPrice * (discountPercent / 100m));
        }
    }
}
