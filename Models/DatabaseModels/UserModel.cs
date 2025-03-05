using ITStepFinalProject.Models.Controller;

namespace ITStepFinalProject.Models.DatabaseModels {
    public class UserModel {

        public int Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public string? Image { get; set; }

        public string? Notes { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; } 
        public string? State { get; set; } 
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserModel() {}

        public UserModel(string email, string password) {
            Email = email;
            Password = password;
        }

        public UserModel(RegisterUserModel registerUserModel)
        {
            PhoneNumber = registerUserModel.PhoneNumber;
            Username = registerUserModel.Username;
            Notes = registerUserModel.Notes;
            Email = registerUserModel.Email;
            Password = registerUserModel.Password;
            Address = registerUserModel.Address;
            City = registerUserModel.City;
            State = registerUserModel.State;
            Country = registerUserModel.Country;
            Image = registerUserModel.Image;
        }
    }
}
