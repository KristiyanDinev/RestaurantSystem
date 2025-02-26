using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models;
using System.Security.Claims;

namespace ITStepFinalProject.Utils
{
    public class UserUtils
    {
        private EncryptionHandler _encryptionHandler;
        private JWTHandler _jwtHandler;

        public readonly string authHeader = "Auth";
        public UserUtils(EncryptionHandler encryptionHandler, JWTHandler jwtHandler)
        {
            _encryptionHandler = encryptionHandler;
            _jwtHandler = jwtHandler;
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

        public async Task<UserModel?> GetUserModelFromAuth(HttpContext context)
        {
            Dictionary<string, object>? claims = await _jwtHandler.VerifyJWT(
                   _encryptionHandler.Decrypt(context.Request.Cookies[authHeader]));

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
            userModel.Address = claims["Address"].ToString();
            userModel.City = claims["City"].ToString();
            userModel.State = claims["State"].ToString();
            userModel.Country = claims["Country"].ToString();
            userModel.PhoneNumber = claims["PhoneNumber"].ToString();
            userModel.Password = claims["Password"].ToString();
            return userModel;
        }

        public List<RestorantAddressModel> GetRestorantsForUser(UserModel user)
        {
            List<RestorantAddressModel> resturantAddressModels = new List<RestorantAddressModel>();
            // of the user by himself

            // get resurant addresses that serve to user address
            foreach (RestorantAddressModel addressOnServer in Program.resturantAddresses)
            {
                // restorant address - the location of the restorant from where they serve
                // user addres - the location of the user which the restoract can serve
                // we check if the restorant can serve the user
                // of the server by the admins
                if (addressOnServer.UserCity.Equals(user.City) &&
                    addressOnServer.UserCountry.Equals(user.Country) &&
                    addressOnServer.UserAddress.StartsWith(user.Address))
                {
                    // is State is provided on user end, but the restorant doesn't serve in that State then skip.
                    // State is optional
                    if (user.State != null && user.State.Length > 0 
                        && !addressOnServer.UserState.Equals(user.State))
                    {
                        continue;
                    }

                    resturantAddressModels.Add(addressOnServer);
                }
            }
            return resturantAddressModels;
        }
    }
}
