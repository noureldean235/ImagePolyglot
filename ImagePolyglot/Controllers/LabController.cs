using Microsoft.AspNetCore.Mvc;
using ImagePolyglot.Models;

namespace ImagePolyglot.Controllers
{
    [ApiController]
    [Route("api/lab")]
    public class LabController : ControllerBase
    {
        private const string CorrectFlag =
            "AWAREX{image_polyglot_execution}";

        [HttpPost("submit-flag")]
        public IActionResult SubmitFlag([FromBody] FlagSubmission request)
        {
            if (request.Flag != CorrectFlag)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Incorrect flag"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Challenge completed successfully!"
            });
        }
    }
}