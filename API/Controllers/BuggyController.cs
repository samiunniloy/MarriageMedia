﻿using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController(DataContext context) : BaseApiController
    {
      //  [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetAuth()
        {
            return "secret text";
        }

        //[Authorize]
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = context.Users.Find(-1);
            if (thing == null) return NotFound();
            return thing;
        }

        [HttpGet("server-error")]
        public ActionResult<AppUser> GetServerError()
        {
            try {
                var thing = context.Users.Find(-1) ?? throw new Exception("A bad thing has happen");
                return thing;
            }
            catch
            {
                return StatusCode(500, "Computer says no!");
            }

           
        }

      //  [Authorize]
        [HttpGet("bad-request")]
        public ActionResult<string> Getbadrequest()
        {
            return BadRequest("This was not a good request");
        }

    }
}
