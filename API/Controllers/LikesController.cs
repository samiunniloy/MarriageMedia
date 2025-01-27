using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController(ILikesRepository likesRepository):BaseApiController
    {

        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult>ToggleLike(int targetUserId)
        {
            var sourceUserId = User.GetUserId();

            if (sourceUserId == targetUserId) return BadRequest("Cant LIked you Own");

            var existingLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);

            if (existingLike == null)
            {
                var like = new UserLike
                {
                    SourceUserId = sourceUserId,
                    TargetUserId=targetUserId
                };
                likesRepository.AddLike(like);
            }
            else
            {
                var like = new UserLike
                {
                    SourceUserId = sourceUserId,
                    TargetUserId = targetUserId
                };
                likesRepository.DeleteLike(like);
            }

            if (await likesRepository.SaveChanges()) return Ok();
            return BadRequest("failed to update lilke");

        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<int>>> GetUserLikeIds()
        {
            return Ok(await likesRepository.GetCurrentUserLikeIds(User.GetUserId()));

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes(string predicate)
        {
            var users = await likesRepository.GetUserLikes(predicate, User.GetUserId());
            return Ok(users);

        }

    }
}
