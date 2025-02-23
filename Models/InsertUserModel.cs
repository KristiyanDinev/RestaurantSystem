namespace ITStepFinalProject.Models
{
    public class InsertUserModel
    {

        public string Username { get; set; }
        public string Password { get; set; }

        public string? Image { get; set; }

        public string? Notes { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FullAddress { get; set; }

        public InsertUserModel() { }
        public InsertUserModel(UserModel model)
        {
            Username = model.Username;
            Password = model.Password;
            Image = model.Image;
            Notes = model.Notes;
            PhoneNumber = model.PhoneNumber;
            Email = model.Email;
            FullAddress = model.FullAddress;
        }
    }
}
