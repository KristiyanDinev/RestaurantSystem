using ITStepFinalProject.Models;
using System.Security.Claims;

namespace ITStepFinalProject.Utils {
    public class Utils {

        public static async Task<bool> IsDateExpired(JWTHandler JWT,
            string? _jwt, string key) {
            if (_jwt == null) {
                return false;
            }
            Dictionary<string, object>? claims = await JWT.VerifyJWT(_jwt);
            if (claims == null) {
                return false;
            }
            try {
                return DateTime.Parse(Convert.ToString(claims[key])) <= DateTime.Now;

            } catch (Exception) {
                return false;
            }
        }

        public static string? Get_JWT_FromCookie(IRequestCookieCollection cookies) {
            if (cookies.TryGetValue("RestorantCookie", out string? jwtD)) {
                return jwtD;
            }
            return null;
        }

        public static byte[] FromStringToUint8Array(string data) {
            string[] dataNumbers = data.Split(",");
            byte[] byteArray = new byte[dataNumbers.Length];
            for (int i = 0; i < dataNumbers.Length; i++) {
                byteArray[i] = Convert.ToByte(dataNumbers[i]);
            }
            return byteArray;
        }

        public static void _handleRememberMe(ref HttpContext context,
            string remeberMe, int Id, JWTHandler jwt) {

            IResponseCookies cookies = context.Response.Cookies;
            cookies.Delete("RestorantCookie");

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("UserId", Id.ToString()));

            // on / off
            if (remeberMe.Equals("off")) {
                claims.Add(new Claim("UserId_ExpirationDate",
                    DateTime.Now.AddDays(1.0).ToString()));
            }

            cookies.Append("RestorantCookie", jwt.GenerateJwtToken(claims));
        }

        public static void _handleReadableData(ref HttpContext context,
            UserModel user) {

            IResponseCookies cookies = context.Response.Cookies;

            cookies.Delete("Username");
            cookies.Delete("Email");
            cookies.Delete("Address");
            cookies.Delete("Notes");
            cookies.Delete("Image");
            cookies.Delete("Phone");

            cookies.Append("Username", user.Username);
            cookies.Append("Email", user.Email);
            cookies.Append("Address", user.Address ?? "");
            cookies.Append("Notes", user.Notes ?? "");
            cookies.Append("Phone", user.PhoneNumber ?? "");
            cookies.Append("Image", user.Image ?? "");
        }

        public static async Task<Dictionary<string, object>?> GetRestoratCookieClaims(HttpContext context,
            JWTHandler jwt) {
            return await jwt.VerifyJWT(
                Get_JWT_FromCookie(context.Request.Cookies));
        }
    }
}
