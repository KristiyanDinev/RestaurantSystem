namespace RestaurantSystem.Models.Form
{
    public class RegisterFormModel
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public IFormFile? Image { get; set; }
        public bool RememberMe { get; set; }

    }
}
