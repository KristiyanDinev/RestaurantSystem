namespace RestaurantSystem.Models.WebSockets
{
    public class RabbitMQOptionsModel
    {
        public required string HostName { get; set; }
        public required int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
