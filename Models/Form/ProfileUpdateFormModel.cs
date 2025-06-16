namespace RestaurantSystem.Models.Form
{
    public class ProfileUpdateFormModel
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public IFormFile? Image { get; set; }
        public bool DeleteImage { get; set; } = false;
    }
}
