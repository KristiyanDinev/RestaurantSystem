using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Services
{
    public class WebSocketDatabaseService
    {
        private readonly DatabaseContext _databaseContext;

        public WebSocketDatabaseService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> AddOrderServerMappingAsync(long OrderId, string ServerId) {
            OrderServerMappingModel? model = await _databaseContext.OrderServerMappings.FirstOrDefaultAsync(m => m.OrderId == OrderId &&
                    m.ServerId.Equals(ServerId));
            if (model != null)
            {
                return true;
            }
            await _databaseContext.OrderServerMappings.AddAsync(new OrderServerMappingModel()
            {
                OrderId = OrderId,
                ServerId = ServerId
            });

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteOrderServerMappingAsync(long OrderId, string ServerId)
        {
            OrderServerMappingModel? model = await _databaseContext.OrderServerMappings
                .FirstOrDefaultAsync(mapping => mapping.OrderId == OrderId && 
                mapping.ServerId.Equals(ServerId));
            if (model == null) { 
                return false;
            }
            _databaseContext.OrderServerMappings.Remove(model);
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAllOrderServerMappingByServerIdAsync(string ServerId)
        {
            List<OrderServerMappingModel> models = await _databaseContext.OrderServerMappings
                .Where(mapping => mapping.ServerId.Equals(ServerId)).ToListAsync();

            foreach (var item in models)
            {
                _databaseContext.OrderServerMappings.Remove(item);
            }
            return await _databaseContext.SaveChangesAsync() >= 0;
        }

        public async Task<bool> DeleteAllOrderServerMappingByOrderIdAsync(long OrderId)
        {
            List<OrderServerMappingModel> models = await _databaseContext.OrderServerMappings
                .Where(mapping => mapping.OrderId == OrderId).ToListAsync();

            foreach (var item in models)
            {
                _databaseContext.OrderServerMappings.Remove(item);
            }
            return await _databaseContext.SaveChangesAsync() >= 0;
        }

        public async Task<OrderServerMappingModel?> GetServerWhichHasListenersForOrderIdAsync(long OrderId)
        {
            OrderServerMappingModel? model = await _databaseContext.OrderServerMappings
                .FirstOrDefaultAsync(mapping => mapping.OrderId == OrderId);
            if (model == null)
            {
                return null;
            }
            return model;
        }
    }
}
