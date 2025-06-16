using RestaurantSystem.Database;

namespace RestaurantSystem.Services
{
    public class AddressService
    {
        private readonly DatabaseContext _databaseContext;
        public AddressService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
    }
}
