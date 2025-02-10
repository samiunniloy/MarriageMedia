using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class accountController(UserManager<AppUser> userManager, ITokenService tokenService,
        IMapper mapper) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

           if (await UserExists(registerDto.Username)) return BadRequest("User Name already Used");

            using var hmac = new HMACSHA512();
           
           var user = mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username;
            

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                knownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                Gender = user.Gender
            };
           
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>>Login(LoginDto loginDto)
        {
            
            var user=await userManager.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.NormalizedUserName == loginDto.Username.ToUpper());


            if (user == null||user.UserName==null) return Unauthorized("Invalid User Name");
            
            return new UserDto
            {

                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                knownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                Gender = user.Gender
            };

        }
        private async Task<bool> UserExists(string username)
        {
            return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
        }
    }
}
