using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Services;
using System.Security.Claims;

namespace RestaurantSystem.Utilities
{
    public class UserUtility
    {
        private EncryptionUtility _encryptionUtility;
        private JWTUtility _jwtUtility;
        private UserService _userService;

        private readonly string authHeader = "Authentication";
        private readonly string userIdClaimKey = "Id";

        public UserUtility(EncryptionUtility encryptionUtility, JWTUtility jwtUtility,
            UserService userService)
        {
            _encryptionUtility = encryptionUtility;
            _jwtUtility = jwtUtility;
            _userService = userService;
        }


        /*
         * Generates a JWT:
         * Claims: "Id": model.Id
         * Based on the `remember me` option (if `remember me` is "off")
         * it specifies an experation date of 1 day.
         */
        public string GenerateAuthBearerHeader_JWT(UserModel model, bool remeberMe)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(userIdClaimKey, model.Id.ToString()));

            return _encryptionUtility.Encrypt(
                _jwtUtility.GenerateJWT(claims,

                remeberMe ? DateTime.Now.AddDays(1.0) : null)
                );
        }

        public void RemoveAuthBearerHeader(HttpContext context)
        {
            context.Response.Cookies.Delete(authHeader);
        }

        public void SetUserAuthBearerHeader(HttpContext context, string jwt)
        {
            RemoveAuthBearerHeader(context);
            context.Response.Cookies.Append(authHeader, jwt);
        }

        public async Task<Dictionary<string, object>?> GetAuthClaimFromJWT(HttpContext context)
        {
            try
            {
                return await _jwtUtility.VerifyJWT(
                   _encryptionUtility.Decrypt(
                       context.Request.Cookies[authHeader]
                       ));

            } catch (Exception)
            {
                return null;
            }
        }

        /*
         * Extracts the JWT from the "Authorization" header.
         * Decrypts the JWT.
         * Verifies the JWT.
         * Gets the JWT claims.
         * 
         * Checks if the claims are valid.
         * Returns the users based on the provided JWT.
         */
        public async Task<UserModel?> GetUserByJWT(HttpContext context)
        {
            try
            {
                Dictionary<string, object>? claims = await GetAuthClaimFromJWT(context);

                if (claims == null || !claims.ContainsKey(userIdClaimKey) ||
                    !int.TryParse(claims[userIdClaimKey].ToString(), out int Id))
                {
                    return null;
                }

                return await _userService.GetUserAsync(Id);

            } catch (Exception)
            {
                return null;
            }
        }


        public async Task<UserModel?> GetStaffUserByJWT(HttpContext context)
        {
            try
            {
                Dictionary<string, object>? claims = await GetAuthClaimFromJWT(context);

                if (claims == null || !claims.ContainsKey(userIdClaimKey) ||
                    !int.TryParse(claims[userIdClaimKey].ToString(), out int Id))
                {
                    return null;
                }

                return await _userService.GetStaffUserAsync(Id);

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
