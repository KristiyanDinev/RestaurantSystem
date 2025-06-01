namespace RestaurantSystem.Models.Form
{
    public class ReservationFormModel
    {
        public required string Notes { get; set; }
        public required int Amount_Of_Adults { get; set; }
        public required int Amount_Of_Children { get; set; }
        public required DateTime At_Date { get; set; }
    }
}
