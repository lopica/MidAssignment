using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MidAssignment.Services
{
    public class JWTServices(IConfiguration configuration) : IJWTServices
    {
        private readonly IConfiguration _configuration = configuration;
        public string GenerateTokenWithPublicKey(string email, string publicKey = "", bool isRefreshToken = false, string role = "User")
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:PrivateKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //List<Claim> claims = [new Claim("PublicKey", publicKey)];
            List<Claim> claims = [];
            if (!isRefreshToken)
            {
                claims.Add(new Claim(ClaimTypes.Email, email));
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
