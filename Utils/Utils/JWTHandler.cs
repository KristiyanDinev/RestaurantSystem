﻿using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ITStepFinalProject.Utils.Utils {
    public class JWTHandler {
        private JwtSecurityTokenHandler jwt;
        private SecurityKey key;
        private TokenValidationParameters tokenValidationParameters;
        private SigningCredentials signingCredentials;

        public JWTHandler(string _secret_key) {
            key = new SymmetricSecurityKey(EncryptionHandler.HashIt(_secret_key)); // - `JWT_SecurityKey` has to be 44 characters in base64 string
            jwt = new JwtSecurityTokenHandler();
            signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            tokenValidationParameters = new TokenValidationParameters() {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                IssuerSigningKey = key // The same key as the one that generate the token
            };
        }

        // <summary>Handles the `exp` - expiration date on the token. It will be empty if none.</summary>
        public string GenerateJwtToken(List<Claim> claims, DateTime? expires) {
            //Claim claim = new Claim("a", "a");
            claims.Add(new Claim("_exp", expires?.ToString() ?? ""));
            return jwt.WriteToken(new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials));
        }


        public async Task<Dictionary<string, object>?> VerifyJWT(string? jwtStr) {
            if (jwtStr == null || jwtStr.Length == 0) {
                return null;
            }

            try {
                TokenValidationResult res = await jwt.ValidateTokenAsync(jwtStr, tokenValidationParameters);
                if (res.IsValid) {
                    Dictionary<string, object> claims = res.Claims.ToDictionary();

                    string exp = claims["_exp"].ToString();
                    if (exp.Length > 0 && DateTime.Parse(exp) <= DateTime.Now)
                    {
                        return null;
                    }

                    return claims;
                }
                return null;

            } catch (Exception) {
                return null;
            }
        }
    }
}
