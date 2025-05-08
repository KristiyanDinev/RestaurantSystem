namespace RestaurantSystem.Models.Form
{
    public class OrderFormModel
    {
        public string? Notes { get; set; }

        public List<int> Dishes { get; set; } = new List<int>();
    }
}
