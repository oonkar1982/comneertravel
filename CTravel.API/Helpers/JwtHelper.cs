using CTravel.API.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CTravel.API.Helpers
{
    public static class JwtHelper
    {
        private static readonly string SecretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
        private static readonly string Issuer = ConfigurationManager.AppSettings["JwtIssuer"];
        private static readonly string Audience = ConfigurationManager.AppSettings["JwtAudience"];

        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // ── Generate Access Token (expires in 10 minutes) ────────────────────
        public static string GenerateAccessToken(string username, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var iat = ((long)(DateTime.UtcNow - UnixEpoch).TotalSeconds).ToString();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,             username),
                new Claim(ClaimTypes.Role,             role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, iat, ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(10),  // ✅ 10 minutes
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ── Generate Refresh Token (random 64-byte string) ───────────────────
        public static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        // ── Validate Access Token (allow expired for refresh flow) ───────────
        public static ClaimsPrincipal ValidateToken(string token,
                                                    bool validateLifetime = true)
        {
            var handler = new JwtSecurityTokenHandler();

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifetime,   // false = allow expired
                ValidateIssuerSigningKey = true,
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(SecretKey)),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                SecurityToken validatedToken;
                return handler.ValidateToken(token, validationParams, out validatedToken);
            }
            catch
            {
                return null;
            }
        }
    }

    public static class TokenStore
    {
        // Key = RefreshToken (GUID string), Value = token data
        private static readonly ConcurrentDictionary<string, RefreshTokenData> _store
            = new ConcurrentDictionary<string, RefreshTokenData>();

        public static void Save(string refreshToken, string username, string role)
        {
            _store[refreshToken] = new RefreshTokenData
            {
                Username = username,
                Role = role,
                ExpiresAt = DateTime.UtcNow.AddDays(1),  // refresh token valid 7 days
                IsRevoked = false
            };
        }

        public static RefreshTokenData Get(string refreshToken)
        {
            RefreshTokenData data;
            return _store.TryGetValue(refreshToken, out data) ? data : null;
        }

        public static void Revoke(string refreshToken)
        {
            RefreshTokenData data;
            if (_store.TryGetValue(refreshToken, out data))
                data.IsRevoked = true;
        }
    }
}
