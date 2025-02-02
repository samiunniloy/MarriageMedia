using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.services
{
    public class TokenServices(IConfiguration config,UserManager<AppUser>userManager) : ITokenService
    {
        public async Task<string> CreateToken(AppUser user)
        {
            var TokenKey = config["TokenKey"] ?? throw new Exception("Cannot access tokenKey from appsetting");
            if (TokenKey.Length < 64) throw new Exception("Your tokenKey needs to be longer");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey));

            if (user.UserName == null) throw new Exception("No username ");

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),            // For username
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // For user ID
        };

            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(100),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
