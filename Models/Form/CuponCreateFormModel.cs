namespace RestaurantSystem.Models.Form
{
    public class CouponCreateFormModel
    {
        public required string Name { get; set; }
        public required string CouponCode { get; set; }
        public required int DiscountPercent { get; set; }
        public required DateOnly ExpDate { get; set; }
    }
}
