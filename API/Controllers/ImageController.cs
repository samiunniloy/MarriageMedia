﻿using API.Extensions;
using API.Rabbit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IRabbitMQService _rabbitMQService;

        public ImagesController(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost]
        public IActionResult UploadImage([FromBody] ImageUploadRequest request)
        {
            if (string.IsNullOrEmpty(request.Base64Image))
            {
                return BadRequest(new { message = "No image data provided." });
            }

            string base64Data = request.Base64Image;
            if (base64Data.Contains(","))
            {
                base64Data = base64Data.Split(',')[1];
            }

            var imageData = Convert.FromBase64String(base64Data);
            var userId = User.GetUserId();

            var processedImage = new ProcessedImage
            {
                UserId = userId,
                Base64Image = base64Data,  
                Date = DateTime.UtcNow
            };

            _rabbitMQService.SendImageForProcessing(processedImage);

            return Ok(new { message = "Image sent for processing" });
        }

        public class ImageUploadRequest
        {
            public string ImageName { get; set; }
            public string Base64Image { get; set; }
        }

        [HttpGet]
        public IActionResult GetAllImages()
        {
            _rabbitMQService.RequestStoredImages();
            var images = _rabbitMQService.ReceiveProcessedImages();
            return Ok(images);
        }
    }
}
