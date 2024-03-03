using Microsoft.IdentityModel.Tokens;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace RecipeAppBackend.Services
{
    public class AuthServices : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("userId", user.Id.ToString()),
                    new Claim("isAdmin", user.Admin.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)
                                        , SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GetUserId(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

            //Extract userId claim
            var userIdClaim = token.Claims.FirstOrDefault(claim => claim.Type == "userId");

            if (userIdClaim != null)
            {
                return userIdClaim.Value;
            }
            else
            {
                return null;
            }
        }

        public bool IsAdmin(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

            //Extract admin claim
            var isAdminClaim = token.Claims.FirstOrDefault(claims => claims.Type == "isAdmin");

            return isAdminClaim.Value == "True" ? true : false;
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
