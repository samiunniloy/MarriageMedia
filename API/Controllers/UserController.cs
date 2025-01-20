using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UserController(IUserRepository userRepository,IMapper mapper) : BaseApiController
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
            if (user == null) {
                return NotFound("name doesnt exist");
            }
            return Ok(user);
        }
    
    }
}
