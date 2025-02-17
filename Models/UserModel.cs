namespace ITStepFinalProject.Models {
    public class UserModel {

        public int __Id { get; set; }

        public string Username { get; set; }
        public string _Password { get; set; }

        public string? Image { get; set; }

        public string? Notes { get; set; }
        public string? PhoneNumber { get; set; }
        public string _Email { get; set; }
        public string FullAddress { get; set; } // address;city;country

    }
}
