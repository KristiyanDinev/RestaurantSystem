using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Database.Utils;
using ITStepFinalProject.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ITStepFinalProject.Utils {
    public class ControllerUtils {

        private EncryptionHandler _encryptionHandler;
        private JWTHandler _jwtHandler;

        public ControllerUtils(EncryptionHandler encryptionHandler, JWTHandler jwtHandler)
        {
            _encryptionHandler = encryptionHandler;
            _jwtHandler = jwtHandler;
        }

        public async Task<UserModel?> GetLoginUserFromCookie(HttpContext context, 
            UserDatabaseHandler userDB)
        {
            try
            {
                UserModel? user = await GetUserModelFromAuth(context);
                return user == null ? null : await userDB.LoginUser(user, false);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async void VerifyLogin(HttpContext context)
        {
        
        }


        public byte[] FromStringToUint8Array(string data) {
            string[] dataNumbers = data.Split(",");
            byte[] byteArray = new byte[dataNumbers.Length];
            for (int i = 0; i < dataNumbers.Length; i++) {
                byteArray[i] = Convert.ToByte(dataNumbers[i]);
            }
            return byteArray;
        }


        public async Task<string> GetFileContent(string path) {
            // path => /login => /dishes
            if (path.Contains('.')) {
                return await File.ReadAllTextAsync($"wwwroot{path}");

            } else {
                return await File.ReadAllTextAsync($"wwwroot{path}/Index.html");
            }

        }


        public void RemoveImage(string image)
        {
            if (File.Exists(image))
            {
                File.Delete(image);
            }
        }

        public async Task<string?> UploadImage(string image)
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


        public List<RestorantAddressModel> GetRestorantsForUser(UserModel user)
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

        public JsonElement GetModelFromSession(ISession session, string modelName)
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

       
        // <summary>The Password in the model can be hashed</summary>
        public string HandleAuth(UserModel model, string remeberMe)
        {
            List<Claim> claims = new List<Claim>();
            foreach (string property in ModelUtils.Get_Model_Property_Names(model))
            {
                claims.Add(new Claim(property,
                    Convert.ToString(ModelUtils.Get_Property_Value(model, property) ?? "")));
            }

            return _encryptionHandler.Encrypt(
                _jwtHandler.GenerateJwtToken(claims,
               
                remeberMe.Equals("off") ? DateTime.Now.AddDays(1.0) : null)
                );
        }

        public async Task<UserModel?> GetUserModelFromAuth(HttpContext context)
        {
            Dictionary<string, object>? claims = await _jwtHandler.VerifyJWT(
                   _encryptionHandler.Decrypt(context.Request.Cookies["Auth"]));

            if (claims == null)
            {
                return null;
            }

            UserModel userModel = new UserModel();
            userModel.Id = int.Parse(claims["Id"].ToString());
            userModel.Notes = claims["Notes"].ToString();
            userModel.Email = claims["Email"].ToString();
            userModel.Username = claims["Username"].ToString();
            userModel.Image = claims["Image"].ToString();
            userModel.FullAddress = claims["FullAddress"].ToString();
            userModel.Password = claims["Password"].ToString();
            return userModel;
        }

        public async Task<IResult> HandleDefaultPage_WithUserModel(string path, HttpContext context)
        {
            try
            {
                UserModel? user = await GetUserModelFromAuth(context);
                if (user == null)
                {
                    return Results.Redirect("/login");
                }

                string FileData = await GetFileContent(path);
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
