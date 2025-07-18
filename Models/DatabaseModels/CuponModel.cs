using System.Collections.ObjectModel;

namespace RestaurantSystem.Models.DatabaseModels {

    public class CuponModel
    {

        public required string Name { get; set; }
        public required string CuponCode { get; set; }
        public required int DiscountPercent { get; set; }
        public required DateOnly ExpirationDate { get; set; }
        
        // navigation
        public Collection<OrderModel> Orders { get; set; } = new Collection<OrderModel>();
    }
}
