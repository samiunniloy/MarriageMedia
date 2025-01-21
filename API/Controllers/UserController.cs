using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
  
   // [Authorize]
    public class UserController(IUserRepository userRepository, IMapper mapper) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMemberAsyunc()
        {
            var users = await userRepository.GetMemberAsync();

            return Ok(users);
        }
        [HttpGet("username")]
        public async Task<ActionResult<MemberDto>> GetMemberAsync(string username)
        {
            var user = await userRepository.GetMemberAsync(username);
            if (user == null)
            {
                return NotFound("name doesnt exist");
            }
            return Ok(user);
        }
        [HttpPut]

        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (username == null) return BadRequest("No Username Found in Token");

            var user = await userRepository.GetUserByUsernameAsync(username);
            if (user == null) return BadRequest("CouldNot Find user");
            mapper.Map(memberUpdateDto, user);
           // Console.WriteLine(user);
            userRepository.update(user);
            if (await userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");
        }
    }
}
