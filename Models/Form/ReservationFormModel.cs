namespace RestaurantSystem.Models.Form
{
    public class ReservationFormModel
    {
        public string? Notes { get; set; }
        public int Amount_Of_Adults { get; set; }
        public int Amount_Of_Children { get; set; }
        public DateTime At_Date { get; set; }
    }
}
