using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        public CloudinaryController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }
        [HttpPost("upload-image-avatar")]
        public async Task<IActionResult> UploadImageAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            using var stream = file.OpenReadStream();

            var result = await _cloudinaryService.UploadFileAsync(stream, file.FileName, "avatar-account");
            return Ok(result);
        }
        [HttpPost("upload-image-reading-question")]
        public async Task<IActionResult> UploadImageReadingQuestion(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            using var stream = file.OpenReadStream();

            var result = await _cloudinaryService.UploadFileAsync(stream, file.FileName, "reading-question-images");
            return Ok(result);
        }
        [HttpPost("upload-image-class")]
        public async Task<IActionResult> UploadImageClass(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            using var stream = file.OpenReadStream();

            var result = await _cloudinaryService.UploadFileAsync(stream, file.FileName, "class-images");
            return Ok(result);
        }
        [HttpPost("upload-image-test-section")]
        public async Task<IActionResult> UploadImageTestSection(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            using var stream = file.OpenReadStream();

            var result = await _cloudinaryService.UploadFileAsync(stream, file.FileName, "test-section-images");
            return Ok(result);
        }
        [HttpPost("upload-image-question")]
        public async Task<IActionResult> UploadImageQuestion(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            using var stream = file.OpenReadStream();

            var result = await _cloudinaryService.UploadFileAsync(stream, file.FileName, "test-question-images");
            return Ok(result);
        }
        [HttpPost("upload-image-mcq-option")]
        public async Task<IActionResult> UploadImageMCQOption(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            using var stream = file.OpenReadStream();

            var result = await _cloudinaryService.UploadFileAsync(stream, file.FileName, "test-mcq-option-images");
            return Ok(result);
        }
    }
}
