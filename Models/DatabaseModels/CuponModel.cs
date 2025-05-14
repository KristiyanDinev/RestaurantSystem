namespace RestaurantSystem.Models.DatabaseModels {

    public class CuponModel {

        public required string Name { get; set; }
        public required string CuponCode { get; set; }
        public required decimal DiscountPercent { get; set; }
        public required DateTime ExpirationDate { get; set; }
    }
}
