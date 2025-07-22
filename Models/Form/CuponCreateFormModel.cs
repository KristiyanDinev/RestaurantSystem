namespace RestaurantSystem.Models.Form
{
    public class CuponCreateFormModel
    {
        public required string Name { get; set; }
        public required string CuponCode { get; set; }
        public required int DiscountPercent { get; set; }
        public required DateOnly ExpDate { get; set; }
    }
}
