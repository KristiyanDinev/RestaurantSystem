using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ITStepFinalProject.Utils {
    public class JWTHandler {
        public string secret_key;
        private JwtSecurityTokenHandler jwt;
        private SecurityKey key;
        private TokenValidationParameters tokenValidationParameters;
        private SigningCredentials signingCredentials;

        public JWTHandler(string _secret_key) {
            secret_key = _secret_key;
            key = new SymmetricSecurityKey(Convert.FromBase64String(secret_key));
            jwt = new JwtSecurityTokenHandler();
            signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            tokenValidationParameters = new TokenValidationParameters() {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                IssuerSigningKey = key // The same key as the one that generate the token
            };
        }

        public string GenerateJwtToken(List<Claim> claims) {
            //Claim claim = new Claim("a", "a");

            return jwt.WriteToken(new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials));
        }


        public async Task<Dictionary<string, object>?> VerifyJWT(string? jwtStr) {
            if (jwtStr == null) {
                return null;
            }
            try {
                TokenValidationResult res = await jwt.ValidateTokenAsync(jwtStr, tokenValidationParameters);
                if (res.IsValid) { 
                    return res.Claims.ToDictionary();
                }
                return null;

            } catch (Exception) {
                return null;
            }
        }
    }
}
