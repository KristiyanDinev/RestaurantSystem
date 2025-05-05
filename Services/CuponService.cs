using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class CuponService
    {
        private DatabaseContext _databaseManager;

        public CuponService(DatabaseContext databaseManager)
        {
            _databaseManager = databaseManager;
        }


        public async void DeleteCupon(string cuponCode)
        {
            CuponModel? cupon = await GetCuponByCode(cuponCode);

            if (cupon == null)
            {
                return;
            }

            _databaseManager.Cupons.Remove(cupon);

            await _databaseManager.SaveChangesAsync();
        }

        public async Task<CuponModel?> GetCuponByCode(string cuponCode)
        {
            return await _databaseManager.Cupons.FirstOrDefaultAsync(
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

            await _databaseManager.Cupons.AddAsync(cupon);

            await _databaseManager.SaveChangesAsync();

            return cupon;
        }
    }
}
