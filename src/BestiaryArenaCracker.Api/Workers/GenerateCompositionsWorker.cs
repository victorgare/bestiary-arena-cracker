using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;

namespace BestiaryArenaCracker.Api.Workers
{
    public class GenerateCompositionsWorker(
        IRoomConfigProvider roomConfigProvider,
        IServiceProvider serviceProvider,
        ILogger<GenerateCompositionsWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var room in roomConfigProvider.Rooms)
            {
                logger.LogInformation("Generating compositions for room {RoomId}", room.Id);
                using var scope = serviceProvider.CreateScope();
                var compositionService = scope.ServiceProvider.GetRequiredService<ICompositionService>();
                await compositionService.GenerateAllCompositionsForRoomAsync(room, stoppingToken);
            }
            logger.LogInformation("Composition generation completed.");
        }
    }
}
