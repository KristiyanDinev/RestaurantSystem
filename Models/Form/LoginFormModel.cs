namespace RestaurantSystem.Models.Form
{
    public class LoginFormModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
