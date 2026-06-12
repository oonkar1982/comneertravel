using CTravel.API.Filters;
using CTravel.API.Helpers;
using CTravel.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CTravel.API.Controllers
{

    
    [ApiKeyAuthFilter]
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        // ── POST api/auth/login ───────────────────────────────────────────────
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public IHttpActionResult Login([FromBody] LoginRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username))
                return BadRequest("Username and Password are required.");

            string role;
            if (model.Username == "admin" && model.Password == "Admin@123") role = "Admin";
            else if (model.Username == "user" && model.Password == "User@123") role = "User";
            else return Content(
            HttpStatusCode.Unauthorized,
            new { Message = "Please verfiy Username and Password are required." }
        );

            var accessToken = JwtHelper.GenerateAccessToken(model.Username, role);
            var refreshToken = JwtHelper.GenerateRefreshToken();

            // Save refresh token
            TokenStore.Save(refreshToken, model.Username, role);

            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Username = model.Username,
                Role = role
            });
        }

        // ── POST api/auth/refresh ─────────────────────────────────────────────
        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        public IHttpActionResult Refresh([FromBody] RefreshTokenRequest model)
        {
            if (model == null ||
                string.IsNullOrWhiteSpace(model.AccessToken) ||
                string.IsNullOrWhiteSpace(model.RefreshToken))
                return BadRequest("AccessToken and RefreshToken are required.");

            // 1. Validate access token signature (skip lifetime check)
            var principal = JwtHelper.ValidateToken(model.AccessToken,
                                                    validateLifetime: false);
            if (principal == null)
                return Content(System.Net.HttpStatusCode.Unauthorized,
                               "Invalid access token.");

            // 2. Validate refresh token
            var storedToken = TokenStore.Get(model.RefreshToken);

            if (storedToken == null)
                return Content(System.Net.HttpStatusCode.Unauthorized,
                               "Refresh token not found.");

            if (storedToken.IsRevoked)
                return Content(System.Net.HttpStatusCode.Unauthorized,
                               "Refresh token has been revoked.");

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                return Content(System.Net.HttpStatusCode.Unauthorized,
                               "Refresh token has expired. Please login again.");

            // 3. Revoke old refresh token (rotation — one-time use)
            TokenStore.Revoke(model.RefreshToken);

            // 4. Issue new token pair
            var newAccessToken = JwtHelper.GenerateAccessToken(
                                      storedToken.Username, storedToken.Role);
            var newRefreshToken = JwtHelper.GenerateRefreshToken();

            TokenStore.Save(newRefreshToken, storedToken.Username, storedToken.Role);

            return Ok(new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Username = storedToken.Username,
                Role = storedToken.Role
            });
        }

        // ── POST api/auth/logout ──────────────────────────────────────────────
        [HttpPost]
        [Route("logout")]
        [AllowAnonymous]
        public IHttpActionResult Logout([FromBody] RefreshTokenRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.RefreshToken))
                return BadRequest("RefreshToken is required.");

            TokenStore.Revoke(model.RefreshToken);
            return Ok(new { Message = "Logged out successfully." });
        }
    }
}