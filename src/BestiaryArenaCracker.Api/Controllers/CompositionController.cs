using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;
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
            return Ok(result);
        }
    }
}
