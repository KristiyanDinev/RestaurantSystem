namespace RestaurantSystem.Models.DatabaseModels {
    public class OrderModel {

        public int Id { get; set; }
        public string CurrentStatus { get; set; }
        public string? Notes { get; set; }
        public DateTime OrderedAt { get; set; }
        public decimal TotalPrice { get; set; }


        public int UserModelId { get; set; }
        public UserModel UserModel { get; set; }



        public int RestaurantModelId { get; set; }
        public RestaurantModel RestaurantModel { get; set; }
    }
}
