using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using Microsoft.AspNetCore.Mvc;

namespace BestiaryArenaCracker.Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class DashboardsController(IRoomConfigProvider roomConfigProvider) : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
