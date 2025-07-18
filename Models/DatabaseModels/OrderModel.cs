using RestaurantSystem.Enums;

namespace RestaurantSystem.Models.DatabaseModels {
    public class OrderModel {

        public long Id { get; set; }
        public required OrderStatusEnum CurrentStatus { get; set; }
        public string? Notes { get; set; }
        public DateTime OrderedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string? TableNumber { get; set; }

        public string? CuponCode { get; set; }
        public CuponModel Cupon { get; set; } = null!;


        public required long UserId { get; set; }
        public UserModel User { get; set; } = null!;



        public required int RestaurantId { get; set; }
        public RestaurantModel Restaurant { get; set; } = null!;


        public long? UserAddressId { get; set; }
        public AddressModel UserAddress { get; set; } = null!;


        // navigation
        public ICollection<OrderedDishesModel> OrderedDishes { get; set; } = new List<OrderedDishesModel>();
        public DeliveryModel Delivery { get; set; } = null!;
    }
}
