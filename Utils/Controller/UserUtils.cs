using ITStepFinalProject.Database.Handlers;
using ITStepFinalProject.Models.DatabaseModels;
using ITStepFinalProject.Utils.Utils;
using System.Security.Claims;

namespace ITStepFinalProject.Utils.Controller
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
            foreach (string property in ObjectUtils.Get_Model_Property_Names(model))
            {
                claims.Add(new Claim(property,
                    Convert.ToString(ObjectUtils.Get_Property_Value(model, property) ?? "")));
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

        
    }
}
