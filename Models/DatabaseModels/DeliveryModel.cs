namespace RestaurantSystem.Models.DatabaseModels
{
    public class DeliveryModel
    {
        public int Id { get; set; }

        public required int UserId { get; set; }
        public UserModel User { get; set; } = null!;

        public required int OrderId { get; set; }
        public OrderModel Order { get; set; } = null!;
    }
}
