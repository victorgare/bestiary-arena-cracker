using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;

namespace BestiaryArenaCracker.ApplicationCore.Services
{
    public class CompositionService(IRoomConfigProvider roomConfigProvider) : ICompositionService
    {
        public Task FindCompositionAsync()
        {
            var firstRoom = roomConfigProvider.Rooms.FirstOrDefault();

            return Task.CompletedTask;
        }
    }
}
