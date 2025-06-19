using RestaurantSystem.Enums;

namespace RestaurantSystem.Models.DatabaseModels
{
    public class ReservationModel
    {
        public int Id { get; set; }
        public string? Notes { get; set; }
        public required ReservationStatusEnum CurrentStatus { get; set; }
        public decimal TotalPrice { get; set; } // default 0.00


        public required long UserId { get; set; }
        public UserModel User { get; set; } = null!;


        public required int RestaurantId { get; set; }
        public RestaurantModel Restaurant { get; set; } = null!;



        public int Amount_Of_Adults { get; set; }
        public int Amount_Of_Children { get; set; }
        public DateTime At_Date { get; set; }
        public DateTime Created_At { get; set; } // default
    }
}
