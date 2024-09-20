using log_food_api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace log_food_api.Services
{
    public static class TokenService
    {
        public static dynamic GenerateToken(string clogin, string ccpf)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                     new Claim(ClaimTypes.Name, clogin.ToString()),
                    new Claim(ClaimTypes.Actor, ccpf.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var ctoken = tokenHandler.WriteToken(token);

            return new
            {
                token = ctoken,
                Expires = tokenDescriptor.Expires
            };
        }
    }
}