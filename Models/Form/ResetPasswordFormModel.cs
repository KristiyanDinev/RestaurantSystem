namespace RestaurantSystem.Models.Form
{
    public class ResetPasswordFormModel
    {
        public required string Email { get; set; }
        public required string Code { get; set; }
        public required string NewPassword { get; set; }
    }
}
