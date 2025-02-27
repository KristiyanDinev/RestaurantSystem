using ITStepFinalProject.Models.Controller;
using ITStepFinalProject.Models.DatabaseModels;

namespace ITStepFinalProject.Models.DatabaseModels.ModifingDatabaseModels
{
    public class InsertUserModel
    {

        public string Username { get; set; }
        public string Password { get; set; }

        public string? Image { get; set; }

        public string? Notes { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string? State { get; set; }
        public string Country { get; set; }

        public InsertUserModel() { }
        public InsertUserModel(UserModel model)
        {
            Username = model.Username;
            Password = model.Password;
            Image = model.Image;
            Notes = model.Notes;
            PhoneNumber = model.PhoneNumber;
            Email = model.Email;
            City = model.City;
            Address = model.Address;
            State = model.State;
            Country = model.Country;
        }

        public InsertUserModel(RegisterUserModel registerUserModel)
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
