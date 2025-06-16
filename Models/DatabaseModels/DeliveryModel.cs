namespace RestaurantSystem.Models.DatabaseModels
{
    public class DeliveryModel
    {
        public long Id { get; set; }

        public required long UserId { get; set; }
        public UserModel User { get; set; } = null!;

        public required long OrderId { get; set; }
        public OrderModel Order { get; set; } = null!;
    }
}
