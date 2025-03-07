﻿using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace API.Controllers
{
  
   [Authorize]
    public class UserController(IUserRepository cachedRepository, IMapper mapper,
        IPhotoService photoService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMemberAsyunc([FromQuery]UserParams userParams)
        {

           userParams.CurrentUsername = User.GetUsername();
         //   userParams.CurrentUsername=User.GetUsername();

            var users = await cachedRepository.GetMemberAsync(userParams);

            Response.AddPaginationHeader(users);

            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetMemberAsync(string username)
        {
            var name = GetStringBeforeBracket(username);

            var user = await cachedRepository.GetMemberAsync(name);
            if (user == null)
            {
                return NotFound("name doesnt exist");
            }
            return Ok(user);
        }
        [HttpPut]

        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();

            if (username == null) return BadRequest("No Username Found in Token");

            var user = await cachedRepository.GetUserByUsernameAsync(username);
            if (user == null) return BadRequest("CouldNot Find user");
            mapper.Map(memberUpdateDto, user);
            // Console.WriteLine(user);
            cachedRepository.update(user);
            return Ok();
            //if (await userRepository.SaveAllAsync()) return NoContent();
            //return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded");

            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // For testing only - remove in production
            //username = "lisa";

            if (username == null)
                return Unauthorized("No Username Found in Token");

            var user = await cachedRepository.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound("Could not find user");

            var result = await photoService.AddPhotoAsync(file);
            if (result.Error != null)
                return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                IsMain = user.Photos.Count == 0
            };

            user.Photos.Add(photo);

            if (await cachedRepository.SaveAllAsync())
            {
                var photoDto = mapper.Map<PhotoDto>(photo);

                // Option 1: Return Created with relative path
                return Created($"api/users/{username}", photoDto);

                // Option 2: Return Ok with the photo
                // return Ok(photoDto);

                // Option 3: If you want to use CreatedAtAction, use this format:
                // return CreatedAtAction(
                //     actionName: nameof(GetMember),
                //     routeValues: new { username },
                //     value: photoDto
                // );
            }

            return BadRequest("Problem adding photo");
        }
        static string GetStringBeforeBracket(string input)
        {
            int index = input.IndexOf('[');
            return index != -1 ? input.Substring(0, index) : input;
        }

    }
}
