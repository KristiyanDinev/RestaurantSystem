namespace RestaurantSystem.Models.DatabaseModels
{
    public class ReservationModel
    {
        public int Id { get; set; }
        public string? Notes { get; set; }
        public required string CurrentStatus { get; set; }


        public int UserId { get; set; }
        public UserModel User { get; set; }


        public int RestaurantId { get; set; }
        public RestaurantModel Restaurant { get; set; }



        public int Amount_Of_Adults { get; set; }
        public int Amount_Of_Children { get; set; }
        public DateTime At_Date { get; set; }
        public DateTime Created_At { get; set; } // default
    }
}
