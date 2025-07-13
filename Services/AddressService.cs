using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Database;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Models.Form;
using RestaurantSystem.Utilities;

namespace RestaurantSystem.Services
{
    public class AddressService
    {
        private readonly DatabaseContext _databaseContext;

        public AddressService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<AddressModel>> GetUserAddressesAsync(long user_id, int page = -1)
        {
            if (page < 0)
            {
                return await _databaseContext.Addresses
                    .Where(a => a.UserId == user_id)
                    .ToListAsync();
            }

            return await Utility.GetPageAsync<AddressModel>(_databaseContext.Addresses
                .Where(a => a.UserId == user_id).AsQueryable(), page).ToListAsync();
        }

        public async Task<bool> AddAddressAsync(long user_id, AddAddressFormModel addAddressForm)
        {
            await _databaseContext.AddAsync(new AddressModel
            {
                Country = addAddressForm.Country,
                State = addAddressForm.State,
                City = addAddressForm.City,
                Address = addAddressForm.Address,
                PhoneNumber = addAddressForm.PhoneNumber,
                PostalCode = addAddressForm.PostalCode,
                Notes = addAddressForm.Notes,
                UserId = user_id
            });

            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<AddressModel?> GetAddressByIdAsync(long address_id)
        {
            return await _databaseContext.Addresses
                .FirstOrDefaultAsync(a => a.Id == address_id);
        }

        public async Task<bool> DeleteAddressAsync(long address_id)
        {
            AddressModel? address = await GetAddressByIdAsync(address_id);
            if (address == null)
            {
                return false;
            }
            _databaseContext.Addresses.Remove(address);
            return await _databaseContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAddressAsync(AddressUpdateFormModel addressUpdateForm)
        {
            AddressModel? address = await GetAddressByIdAsync(addressUpdateForm.Id);
            if (address == null)
            {
                return false;
            }

            address.Country = addressUpdateForm.Country;
            address.State = addressUpdateForm.State;
            address.City = addressUpdateForm.City;
            address.Address = addressUpdateForm.Address;
            address.PhoneNumber = addressUpdateForm.PhoneNumber;
            address.PostalCode = addressUpdateForm.PostalCode;
            address.Notes = addressUpdateForm.Notes;

            return await _databaseContext.SaveChangesAsync() > 0;
        }
    }
}
