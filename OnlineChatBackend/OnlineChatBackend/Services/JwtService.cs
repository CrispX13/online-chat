using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineChatBackend.Models;
using OnlineChatBackend.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineChatBackend.Services
{ 
    public class JwtService(IOptions<AuthSettings> options)
    {
        public string GenerateToken(Contact account)
        {
            var claims = new List<Claim>
            {
                new Claim("userName", account.Name),
                new Claim("id", account.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString())
            };
            var jwtToken = new JwtSecurityToken(
                expires: DateTime.UtcNow.Add(options.Value.Expires),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
