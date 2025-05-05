namespace RestaurantSystem.Models.DatabaseModels {
    public class OrderModel {

        public int Id { get; set; }
        public string CurrentStatus { get; set; }
        public string? Notes { get; set; }
        public DateTime OrderedAt { get; set; }
        public decimal TotalPrice { get; set; }


        public int UserId { get; set; }
        public UserModel User { get; set; }



        public int RestaurantId { get; set; }
        public RestaurantModel Restaurant { get; set; }


        // navigation
        public ICollection<OrderedDishesModel> OrderedDishes { get; set; }
    }
}
