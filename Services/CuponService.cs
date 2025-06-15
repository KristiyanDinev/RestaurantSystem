using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

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

        public async Task<CuponModel?> GetCuponByCodeAsync(string cuponCode)
        {
            return await _databaseContext.Cupons.FirstOrDefaultAsync(
                c => c.CuponCode == cuponCode);
        }

        public async Task<CuponModel?> CreateCuponAsync(string cuponCode, string name,
            DateOnly expirationDate, int discountPercent)
        {
            CuponModel cupon = new CuponModel() { 
                Name = name,
                CuponCode = cuponCode,
                DiscountPercent = discountPercent,
                ExpirationDate = expirationDate
            };

            await _databaseContext.Cupons.AddAsync(cupon);

            return await _databaseContext.SaveChangesAsync() > 0 ? cupon : null;
        }

        public decimal HandleCuponDiscount(int discountPercent, decimal totalPrice)
        {
            return totalPrice - (totalPrice * (discountPercent / 100));
        }
    }
}
