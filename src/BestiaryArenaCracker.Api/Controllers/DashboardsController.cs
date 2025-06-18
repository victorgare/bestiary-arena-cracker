using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestiaryArenaCracker.Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class DashboardsController(IDashboardService dashboardService) : ControllerBase
    {
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var dashboard = await dashboardService.GetSummaryAsync();
            return Ok(dashboard);
        }

        [HttpGet("room/{roomId}/details")]
        public async Task<IActionResult> GetRoomDetails(string roomId)
        {
            var dashboard = await dashboardService.GetRoomDetailsAsync(roomId);
            return Ok(dashboard);
        }

        [HttpGet("timespan")]
        public async Task<IActionResult> GetTimespan([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var dashboard = await dashboardService.GetTimespanAsync(start, end);
            return Ok(dashboard);
        }
    }
}
