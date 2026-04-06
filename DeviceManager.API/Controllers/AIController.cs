using Microsoft.AspNetCore.Mvc;
using DeviceManager.API.Services;

namespace DeviceManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public AIController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("generate-description")]
        public async Task<ActionResult> GenerateDescription([FromBody] GenerateDescriptionRequest request)
        {
            try
            {
                Console.WriteLine($"Generating description for: {request.Name}");
        
                var description = await _geminiService.GenerateDescription(
                request.Name, request.Manufacturer, request.Type,
                request.OperatingSystem, request.Processor, request.RAM);

                Console.WriteLine($"Generated: {description}");
        
                return Ok(new { description });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return StatusCode(500, new { message = "Failed to generate description.", error = ex.Message });
            }
        }
    }

    public class GenerateDescriptionRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public string Processor { get; set; } = string.Empty;
        public int RAM { get; set; }
    }
}