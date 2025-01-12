using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class TokenServices(IConfiguration config) : ITokenService
    {
        public string CreateToken(AppUser user)
        {
            var tokenkey = config["TokenKey"] ?? throw new Exception("Cannot access tokenKey from appsetting");
            if (tokenkey.Length < 64) throw new Exception("Your tokenKey needs tp be longer");
            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenkey));


            //claims are the information we want to store in the token
            //we are storing the username in the token
            //clamins holo ki ki access dite parbe token theke
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.UserName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
          //  Console.WriteLine(token);
            return tokenHandler.WriteToken(token);
        }
    }
}
