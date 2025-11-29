namespace RestaurantSystem.Models.WebSockets
{
    /// Message structure for WebSocket distribution across servers
    public class WebSocketDistributionMessageModel
    {
        public long OrderId { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public object? Data { get; set; }
        public string TargetServerId { get; set; } = string.Empty;
        public string SourceServerId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
