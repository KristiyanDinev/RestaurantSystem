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

        public UserUtility(EncryptionUtility encryptionUtility, JWTUtility jwtUtility,
            UserService userService)
        {
            _encryptionUtility = encryptionUtility;
            _jwtUtility = jwtUtility;
            _userService = userService;
        }

        /*
         * A clean and easy way to extract the JWT from the header.
         */

        private string? _ExtractJWTFromHeader(string? header)
        {
            return header != null && header.StartsWith("Bearer ") ? header.Substring(7) : null;
        }

        /*
         * Generates a JWT:
         * Claims: "Id": model.Id
         * Based on the `remember me` option (if `remember me` is "off")
         * it specifies an experation date of 1 day.
         */
        public string GenerateAuthBearerHeader_JWT(UserModel model, string remeberMe)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Id", model.Id.ToString()));

            return _encryptionUtility.Encrypt(
                _jwtUtility.GenerateJWT(claims,

                remeberMe.Equals("off") ? DateTime.Now.AddDays(1.0) : null)
                );
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
                Dictionary<string, object>? claims = await _jwtUtility.VerifyJWT(
                   _encryptionUtility.Decrypt(
                       _ExtractJWTFromHeader(context.Request.Cookies[authHeader]
                       )));

                if (claims == null || !claims.ContainsKey("Id") ||
                    !int.TryParse(claims["Id"].ToString(), out int Id))
                {
                    return null;
                }

                return await _userService.GetUser(Id);

            } catch (Exception e)
            {
                return null;
            }
        }
    }
}
