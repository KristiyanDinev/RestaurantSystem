using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Services
{
    public class CuponService
    {
        private DatabaseContext _databaseContext;

        public CuponService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }


        public async Task<bool> DeleteCuponAsync(string cuponCode)
        {
            CuponModel? cupon = await GetCuponByCodeAsync(cuponCode);
            if (cupon == null)
            {
                return false;
            }
            _databaseContext.Cupons.Remove(cupon);
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<List<CuponModel>> GetCuponsAsync(int page)
        {
            return await Utility.GetPageAsync<CuponModel>(
                _databaseContext.Cupons
                .OrderByDescending(c => c.DiscountPercent)
                .AsQueryable(), page
            ).ToListAsync();
        }

        public async Task<CuponModel?> GetCuponByCodeAsync(string cuponCode)
        {
            return await _databaseContext.Cupons.FirstOrDefaultAsync(
                c => c.CuponCode.Equals(cuponCode));
        }

        public async Task<bool> EditCuponAsync(CuponCreateFormModel model)
        {
            CuponModel? cupon = await GetCuponByCodeAsync(model.CuponCode);
            if (cupon == null)
            {
                return false;
            }
            cupon.Name = model.Name;
            cupon.DiscountPercent = model.DiscountPercent;
            cupon.ExpirationDate = model.ExpDate;
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateCuponAsync(CuponCreateFormModel model)
        {
            await _databaseContext.Cupons.AddAsync(new CuponModel()
            {
                Name = model.Name,
                CuponCode = model.CuponCode,
                DiscountPercent = model.DiscountPercent,
                ExpirationDate = model.ExpDate
            });
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public decimal HandleCuponDiscount(int discountPercent, decimal totalPrice)
        {
            return totalPrice - (totalPrice * (discountPercent / 100m));
        }
    }
}
