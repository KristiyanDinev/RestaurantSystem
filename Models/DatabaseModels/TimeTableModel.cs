namespace RestaurantSystem.Models.DatabaseModels
{
    public class TimeTableModel
    {
        public required int RestuarantId { get; set; }
        public RestaurantModel Restuarant { get; set; } = null!;

        public required string UserAddress { get; set; }
        public required string UserCity { get; set; }
        public string? UserState { get; set; }
        public required string UserCountry { get; set; }

        public required string AvrageDeliverTime { get; set; }
    }
}
