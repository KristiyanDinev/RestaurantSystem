namespace RestaurantSystem.Models.DatabaseModels
{
    public class OrderServerMappingModel
    {
        public long Id { get; set; }
        public required long OrderId { get; set; }
        public required string ServerId { get; set; }
    }
}
