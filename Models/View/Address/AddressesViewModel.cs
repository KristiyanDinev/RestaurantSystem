using RestaurantSystem.Models.DatabaseModels;

namespace RestaurantSystem.Models.View.Address
{
    public class AddressesViewModel
    {
        public required UserModel User { get; set; }
        public required int Page { get; set; } = 1;
        public required List<AddressModel> Addresses { get; set; } = new List<AddressModel>();
    }
}
