using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;
using BestiaryArenaCracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BestiaryArenaCracker.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompositionController(ICompositionService compositionService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetComposition([FromQuery] int count = 1)
        {
            var results = await compositionService.FindCompositionAsync(count);

            if (results.Count == 0)
            {
                return NoContent();
            }

            return Ok(results);
        }

        [HttpPost("{compositionId}")]
        public async Task<IActionResult> Results(int compositionId, CompositionResultsEntity[] compositions)
        {
            await compositionService.AddResults(compositionId, compositions);
            return Ok();
        }
    }
}
