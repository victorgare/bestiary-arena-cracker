using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestiaryArenaCracker.Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class DashboardsController(IDashboardService dashboardService) : ControllerBase
    {
        [HttpGet("summary")]
        public async Task<IActionResult> GetSymmary()
        {
            var dashboard = await dashboardService.GetSummaryAsync();
            return Ok(dashboard);
        }
    }
}
