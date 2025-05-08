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


        public async Task<bool> DeleteCupon(string cuponCode)
        {
            CuponModel? cupon = await GetCuponByCode(cuponCode);

            if (cupon == null)
            {
                return false;
            }

            _databaseContext.Cupons.Remove(cupon);

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<CuponModel?> GetCuponByCode(string cuponCode)
        {
            return await _databaseContext.Cupons.FirstOrDefaultAsync(
                c => c.CuponCode == cuponCode);
        }

        public async Task<CuponModel?> CreateCupon(string cuponCode, string name,
            DateTime expirationDate, decimal discountPercent)
        {
            CuponModel cupon = new CuponModel();
            cupon.ExpirationDate = expirationDate;
            cupon.DiscountPercent = discountPercent;
            cupon.CuponCode = cuponCode;
            cupon.Name = name;

            await _databaseContext.Cupons.AddAsync(cupon);

            return await _databaseContext.SaveChangesAsync() > 0 ? cupon : null;
        }
    }
}
