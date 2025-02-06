using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photo.Data;
using Photo.Entity;

namespace Photo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController(IPhotoRepository _photoRepository) : ControllerBase
    {

       
        [HttpGet]
        public async Task<IActionResult> GetPhoto(string userName)
        {

            var photos = await _photoRepository.GetPhoto(userName);

            // If no photos found, return 404 Not Found
            if (photos == null || !photos.Any())
            {
                return Ok(new { Message = "No photos found for the given user." });
            }

            // Return 200 OK with the photos
            return Ok(photos);

        }

        [HttpPost]

        public async Task<IActionResult> SavePhoto(Picture picture)
        {
            await _photoRepository.SavePhoto(picture);
            return Ok("Saved");
        }
    }
}
