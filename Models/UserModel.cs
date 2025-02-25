using System.Numerics;

namespace ITStepFinalProject.Models {
    public class UserModel {

        public int Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public string? Image { get; set; }

        public string? Notes { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FullAddress { get; set; } // address;city;country

        public UserModel() {}

        public UserModel(string email, string password) {
            Email = email;
            Password = password;
        }

        public UserModel(string fulladdress, string? phone, 
            string username, string? notes, string email, string password)
        {
            FullAddress = fulladdress;
            PhoneNumber = phone;
            Username = username;
            Notes = notes;
            Email = email;
            Password = password;
        }
    }
}
