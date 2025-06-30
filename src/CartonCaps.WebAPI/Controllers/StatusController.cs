using Microsoft.AspNetCore.Mvc;

namespace CartonCaps.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/status")]
    public class StatusController : ControllerBase
    {
        /// <summary> Returns the status of the CartonCaps API.</summary>
        /// <returns> A simple status message indicating the API is running.</returns>
        [HttpGet]
        public IActionResult GetStatus()
        {
            // Return a simple status message
            //TODO: This should also check database connectivity, dependencies, etc.
            return Ok(new { Status = "CartonCaps API is running smoothly!" });
        }
    }
}