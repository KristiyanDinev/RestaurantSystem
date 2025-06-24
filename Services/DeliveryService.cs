using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Services
{
    public class DeliveryService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly AddressService _addressService;
        private readonly RestaurantService _restaurantService;

        public DeliveryService(DatabaseContext databaseContext, 
            AddressService addressService,
            RestaurantService restaurantService)
        {
            _databaseContext = databaseContext;
            _addressService = addressService;
            _restaurantService = restaurantService;
        }
        public async Task<DeliveryModel?> GetDeliveryAsync(int user_id)
        {
            return await _databaseContext.Delivery
                .FirstOrDefaultAsync(d => d.UserId == user_id);
        }


        public async Task<bool> AddDeliveryAsync(int user_id, int order_id)
        {
            DeliveryModel delivery = new ()
            {
                UserId = user_id,
                OrderId = order_id
            };

            await _databaseContext.Delivery.AddAsync(delivery);

            return await _databaseContext.SaveChangesAsync() > 0;
        }


        public async Task<bool> RemoveDeliveryAsync(int user_id)
        {
            DeliveryModel? delivery = await GetDeliveryAsync(user_id);
            if (delivery == null)
            {
                return false;
            }

            _databaseContext.Delivery.Remove(delivery);

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<AddressModel?> GetDeliveryAddressCookie(HttpContext context)
        {
            context.Request.Cookies.TryGetValue(Utility.delivery_address_header,
                out string? address_id_str);
            if (!long.TryParse(address_id_str, out long address_id))
            {
                return null;
            }

            return await _addressService.GetAddressByIdAsync(address_id);
        }

        public async Task<RestaurantModel?> GetDeliveryRestaurantCookie(HttpContext context)
        {
            context.Request.Cookies.TryGetValue(Utility.delivery_restaurant_header,
                out string? restaurant_id_str);
            if (!int.TryParse(restaurant_id_str, out int restaurant_id))
            {
                return null;
            }

            return await _restaurantService.GetRestaurantByIdAsync(restaurant_id);
        }
    }
}
