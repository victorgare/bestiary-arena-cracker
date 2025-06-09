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
        public async Task<IActionResult> GetComposition()
        {
            var result = await compositionService.FindCompositionAsync();

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        [HttpPost("{compositionId}")]
        public async Task<IActionResult> Results(int compositionId, CompositionResultsEntity[] compositions)
        {
            await compositionService.AddResults(compositionId, compositions);
            return Ok();
        }
    }
}
