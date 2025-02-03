﻿using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace API.Controllers
{
    public class AdminController(UserManager<AppUser> userManager):BaseApiController
    {

        //[Authorize(Policy = "RequiredAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult<string>> GetUserWithRoles()
        {
            var users = await userManager.Users
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();
            return Ok(users);
        }
      //[Authorize(Policy = "requiredAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult<string>> EditRoles(string username, [FromQuery] string roles)
        {

            if (string.IsNullOrEmpty(roles)) return BadRequest("You need to select roles");

            var selectedRoles = roles.Split(",").ToArray();
            var user = await userManager.FindByNameAsync(username);
            if (user == null) return NotFound("Could not find user");
            var userRoles = await userManager.GetRolesAsync(user);
            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Failed to add to roles");
            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("Failed to remove from roles");
            return Ok(await userManager.GetRolesAsync(user));
        }

        //[Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-tomoderate")]
        public ActionResult<string> GetPhotosForModeration()
        {
            return Ok("Only For Admin");
        }
    }
}
