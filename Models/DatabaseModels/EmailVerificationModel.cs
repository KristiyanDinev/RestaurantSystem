namespace RestaurantSystem.Models.DatabaseModels
{
    public class EmailVerificationModel
    {
        public long Id { get; set; }
        public required string Email { get; set; }
        public required string Code { get; set; }
        public required DateTime ExpiresAt { get; set; }
    }
}
