using RestaurantSystem.Database.Handlers;
using RestaurantSystem.Models.DatabaseModels;
using RestaurantSystem.Utils.Utils;
using System.Security.Claims;

namespace RestaurantSystem.Utils.Controller
{
    public class UserUtils
    {
        private EncryptionHandler _EncryptionHandler;
        private JWTHandler _JWTHandler;
        private UserDatabaseHandler _UserDatabaseHandler;

        public readonly string authHeader = "Authorization";

        public UserUtils(EncryptionHandler encryptionHandler, JWTHandler jwtHandler,
            UserDatabaseHandler UserDatabaseHandler)
        {
            _EncryptionHandler = encryptionHandler;
            _JWTHandler = jwtHandler;
            _UserDatabaseHandler = UserDatabaseHandler;
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
        public string GenerateJWT(UserModel model, string remeberMe)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Id", model.Id.ToString()));

            return _EncryptionHandler.Encrypt(
                _JWTHandler.GenerateJwtToken(claims,

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
            Dictionary<string, object>? claims = await _JWTHandler.VerifyJWT(
                   _EncryptionHandler.Decrypt(
                       _ExtractJWTFromHeader(context.Request.Cookies[authHeader]
                       )));

            if (claims == null || !claims.ContainsKey("Id") ||
                !int.TryParse(claims["Id"].ToString(), out int Id)) {
                return null;
            }

            return await _UserDatabaseHandler.GetUser(Id);
        }
    }
}
