using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using System.Text;
using System.Text.Json;

namespace ITStepFinalProject.Utils {
    public class ControllerUtils {

        public static bool IsDateExpired(ISession session,
           string key) {
            try {
                return DateTime.Parse(session.GetString(key)) <= DateTime.Now;

            } catch (Exception) {
                return false;
            }
        }


        public static byte[] FromStringToUint8Array(string data) {
            string[] dataNumbers = data.Split(",");
            byte[] byteArray = new byte[dataNumbers.Length];
            for (int i = 0; i < dataNumbers.Length; i++) {
                byteArray[i] = Convert.ToByte(dataNumbers[i]);
            }
            return byteArray;
        }

        public static void HandleRememberMe(ref ISession session,
            string remeberMe, int Id) {

            session.SetInt32("UserId", Id);
            if (remeberMe.Equals("off")) {

                session.SetString("UserId_ExpirationDate",
                    DateTime.Now.AddDays(1.0).ToString());
            }
        }



        public static async Task<string> GetFileContent(string path) {
            // path => /login => /dishes
            if (path.Contains('.')) {
                return await File.ReadAllTextAsync($"wwwroot{path}");

            } else {
                return await File.ReadAllTextAsync($"wwwroot{path}/Index.html");
            }

        }


        public static int? IsLoggedIn(ISession session) {
            try {
                return session.GetInt32("UserId");

            } catch (Exception) {
                return null;
            }
        }

        public static void RemoveImage(string image)
        {
            if (File.Exists(image))
            {
                File.Delete(image);
            }
        }

        public static async Task<string?> UploadImage(string image)
        {
            string[] imageParts = image.Split(';');
            string imageName = imageParts[0];
            string imageData = imageParts[1];

            string byteData = Encoding.UTF8.GetString(
                Convert.FromBase64String(imageData));
            if (!(imageName.Contains('\\') ||
                imageName.Contains('/') ||
                imageName.Contains('\'') ||
                byteData.EndsWith(',') ||
                byteData.StartsWith(',')))
            {
                string imgPath = "wwwroot/images/user/" + imageName;
                RemoveImage(imgPath);
                using FileStream fs =
                    File.Create("wwwroot/images/user/" + imageName);
                await fs.WriteAsync(FromStringToUint8Array(byteData));

                return "/images/user/" + imageName;
            }
            return null;
        }

        public static UserModel GetUserModel(string username, string email, string password,
            string fulladdress, string? phone, string? notes)
        {
            UserModel userModel = new UserModel();
            userModel.FullAddress = fulladdress;
            userModel.PhoneNumber = phone;
            userModel.Username = username;
            userModel.Notes = notes;
            userModel.Email = email;
            userModel.Password = password;
            return userModel;
        }


        public static List<RestorantAddressModel> GetRestorantsForUser(UserModel user)
        {
            List<RestorantAddressModel> resturantAddressModels = new List<RestorantAddressModel>();
            // of the user by himself
            string[] parts = user.FullAddress.Split(";");
            string address = parts[0];
            string city = parts[1];
            string state = parts[2];
            string country = parts[3];

            // get resurant addresses that serve to user address
            foreach (RestorantAddressModel addressOnServer in Program.resturantAddresses)
            {
                // restorant address - the location of the restorant from where they serve
                // user addres - the location of the user which the restoract can serve
                // we check if the restorant can serve the user
                // of the server by the admins
                if (addressOnServer.UserCity.Equals(city) &&
                    addressOnServer.UserCountry.Equals(country) &&
                    addressOnServer.UserAddress.StartsWith(address))
                {
                    // is State is provided on user end, but the restorant doesn't serve in that State then skip.
                    // State is optional
                    if (state.Length > 0 && !addressOnServer.UserState.Equals(state))
                    {
                        continue;
                    }

                    resturantAddressModels.Add(addressOnServer);
                }
            }
            return resturantAddressModels;
        }

        public static JsonElement GetModelFromSession(ISession session, string modelName)
        {
            return JsonSerializer.Deserialize<JsonElement>(session.GetString(modelName));
            /*
            string key = modelName+":" + modelID + ":";
            foreach (string property in ModelUtils.Get_Model_Property_Names(model))
            {
                if (ModelUtils.Get_PropertyInfo(model, property).PropertyType is int)
                {
                    ModelUtils.Set_Property_Value(model, property,
                        session.Get(key + property));
                } else
                {
                    ModelUtils.Set_Property_Value(model, property,
                        session.GetString(key + property));
                }
                    
            }
            return model;*/
        }

        public static void SaveModelToSession(ref ISession session,
            string modelName, object model)
        {
            /*
            string key = modelName + ":" + modelID + ":";
            foreach (string property in ModelUtils.Get_Model_Property_Names(model))
            {
                session.SetString(key+property, 
                    Convert.ToString(ModelUtils.Get_Property_Value(model, property) ?? ""));
            }*/
            session.SetString(modelName, JsonSerializer.Serialize(model));
        }

        public static async Task<IResult> HandleDefaultPage_WithUserModel(string path, HttpContext context, 
            UserDatabaseHandler db)
        {
            try
            {
                ISession session = context.Session;
                int? id = IsLoggedIn(context.Session);
                if (id == null)
                {
                    return Results.Redirect("/login");
                }

                string FileData = await GetFileContent(path);

                UserModel user = GetModelFromSession(session, "User").Deserialize<UserModel>();

                FileData = WebHelper.HandleCommonPlaceholders(FileData, "User", [user]);

                return Results.Content(FileData, "text/html");

            }
            catch (Exception)
            {
                return Results.Redirect("/error");
            }
        }
    }
}
