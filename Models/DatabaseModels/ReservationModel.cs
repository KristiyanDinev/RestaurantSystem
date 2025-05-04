namespace RestaurantSystem.Models.DatabaseModels
{
    public class ReservationModel
    {
        public int Id { get; set; }
        public string? Notes { get; set; }
        public string CurrentStatus { get; set; }

        public int UserModelId { get; set; }
        public UserModel UserModel { get; set; }


        public int RestaurantModelId { get; set; }
        public RestaurantModel RestaurantModel { get; set; }



        public int Amount_Of_Adults { get; set; }
        public int Amount_Of_Children { get; set; }
        public DateTime At_Date { get; set; }
        public DateTime Created_At { get; set; } // default
    }
}
