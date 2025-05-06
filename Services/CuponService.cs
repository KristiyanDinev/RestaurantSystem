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


        public async void DeleteCupon(string cuponCode)
        {
            CuponModel? cupon = await GetCuponByCode(cuponCode);

            if (cupon == null)
            {
                return;
            }

            _databaseContext.Cupons.Remove(cupon);

            await _databaseContext.SaveChangesAsync();
        }

        public async Task<CuponModel?> GetCuponByCode(string cuponCode)
        {
            return await _databaseContext.Cupons.FirstOrDefaultAsync(
                c => c.CuponCode == cuponCode);
        }

        public async Task<CuponModel> CreateCupon(string cuponCode, string name,
            DateTime expirationDate, decimal discountPercent)
        {
            CuponModel cupon = new CuponModel();
            cupon.ExpirationDate = expirationDate;
            cupon.DiscountPercent = discountPercent;
            cupon.CuponCode = cuponCode;
            cupon.Name = name;

            await _databaseContext.Cupons.AddAsync(cupon);

            await _databaseContext.SaveChangesAsync();

            return cupon;
        }
    }
}
