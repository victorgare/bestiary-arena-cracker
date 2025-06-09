using BestiaryArenaCracker.ApplicationCore.Services.Composition;
using BestiaryArenaCracker.Domain.Entities;
using BestiaryArenaCracker.Domain.Room;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Services
{
    public interface ICompositionService
    {
        Task<CompositionResult?> FindCompositionAsync();
        Task AddResults(int compositionId, CompositionResultsEntity[] compositions);

        Task<int> CalculatePossibleCompositions(RoomConfig room);
    }
}
