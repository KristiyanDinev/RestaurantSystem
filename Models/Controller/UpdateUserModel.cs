namespace ITStepFinalProject.Models.Controller
{
    public class UpdateUserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Image { get; set; }
        public string? Notes { get; set; }
        public string? PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string? State { get; set; }
        public string Country { get; set; }
        public string DeleteImage { get; set; }
    }
}
