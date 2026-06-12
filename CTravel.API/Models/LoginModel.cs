using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTravel.API.Models
{
   

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class RefreshTokenData
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
    }
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; } = 600;   // 10 minutes in seconds
        public string Username { get; set; }
        public string Role { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}