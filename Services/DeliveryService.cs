using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class DeliveryService
    {
        private readonly DatabaseContext _databaseContext;

        public DeliveryService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public async Task<DeliveryModel?> GetDelivery(int user_id)
        {
            return await _databaseContext.Delivery
                .FirstOrDefaultAsync(d => d.UserId == user_id);
        }


        public async Task<bool> AddDelivery(int user_id, int order_id)
        {
            DeliveryModel delivery = new ()
            {
                UserId = user_id,
                OrderId = order_id
            };

            await _databaseContext.Delivery.AddAsync(delivery);

            return await _databaseContext.SaveChangesAsync() > 0;
        }


        public async Task<bool> RemoveDelivery(int user_id)
        {
            DeliveryModel? delivery = await GetDelivery(user_id);
            if (delivery == null)
            {
                return false;
            }

            _databaseContext.Delivery.Remove(delivery);

            return await _databaseContext.SaveChangesAsync() > 0;
        }
    }
}
