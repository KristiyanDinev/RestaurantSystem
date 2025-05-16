namespace RestaurantSystem.Models.Form
{
    public class OrderFormModel
    {
        public required string Notes { get; set; }
        public required string CuponCode { get; set; }

        public List<int> Dishes { get; set; } = new List<int>();
    }
}
