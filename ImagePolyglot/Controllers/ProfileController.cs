
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using System.Net.Http.Json;

namespace ImagePolyglot.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly HttpClient _httpClient;

        public ProfileController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5100/")
            };
        }

        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            if (avatar.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "File is too large" });

            var extension = Path.GetExtension(avatar.FileName)
                .ToLowerInvariant();

            if (extension != ".jpg" &&
                extension != ".jpeg" &&
                extension != ".png")
            {
                return BadRequest(new
                {
                    message = "Only image extensions are allowed"
                });
            }

            var uploadPath = Path.Combine(
                _environment.ContentRootPath,
                "Uploads"
            );

            Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid() + extension;
            var filePath = Path.Combine(uploadPath, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            return Ok(new
            {
                success = true,
                fileName,
                message = "Image uploaded successfully"
            });
        }

        [HttpGet("metadata/{fileName}")]
        public async Task<IActionResult> GetMetadata(string fileName)
        {
            fileName = Path.GetFileName(fileName);

            var filePath = Path.Combine(
                _environment.ContentRootPath,
                "Uploads",
                fileName
            );

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Image not found" });

            try
            {
                using var image = Image.Load(filePath);

                var exif = image.Metadata.ExifProfile;

                var description = exif?.Values
                    .FirstOrDefault(item =>
                        item.Tag == ExifTag.ImageDescription)
                    ?.GetValue()
                    ?.ToString() ?? "";

                var metadata = exif?.Values.Select(item => new
                {
                    Tag = item.Tag.ToString(),
                    Value = item.GetValue()?.ToString()
                });

                var response = await _httpClient.PostAsJsonAsync(
                    "run",
                    new { Code = description }
                );

                if (!response.IsSuccessStatusCode)
                    return StatusCode(502, new
                    {
                        message = "Runner service failed"
                    });

                var result = await response.Content
                    .ReadFromJsonAsync<RunnerResponse>();

                bool completed = result?.Success == true;

                return Ok(new
                {
                    fileName,
                    width = image.Width,
                    height = image.Height,
                    metadata = metadata ?? Enumerable.Empty<object>(),
                    challengeCompleted = completed,
                    flag = completed
                        ? "AWAREX{image_polyglot_execution}"
                        : null,
                    runnerOutput = result?.Output
                });
            }
            catch (UnknownImageFormatException)
            {
                return BadRequest(new
                {
                    message = "Uploaded file is not a valid image"
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new
                {
                    message = "Runner is not available"
                });
            }
        }
    }

    public class RunnerResponse
    {
        public bool Success { get; set; }
        public string Output { get; set; } = string.Empty;
    }
}
