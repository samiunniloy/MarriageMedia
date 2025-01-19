using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
   
    public class UserController(DataContext context) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {   
            // Task async operation bujhay
            //Ienumerable<AppUser> er jaygay var o use kora jay
            //Ienumerable<AppUser> er jaygay List<AppUser> o use kora jay
            //IEnumerable array type er jnno use hoy
            var users = await context.Users.ToListAsync();
            Console.WriteLine("oikkkkk");
            return Ok(users);
        }
      //  [Authorize]
        [HttpGet("{id:int}")] //api/users/id
        // {id } er jnno dynamic route use kora jay na dile dynamic route hoy na 
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null) {
                return NotFound("Id doesnt exist");
            }
            return Ok(user);
        }
    
    }
}
