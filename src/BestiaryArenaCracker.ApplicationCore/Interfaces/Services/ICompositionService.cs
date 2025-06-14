using BestiaryArenaCracker.ApplicationCore.Services.Composition;
using BestiaryArenaCracker.Domain.Entities;
using BestiaryArenaCracker.Domain.Room;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Services
{
    public interface ICompositionService
    {
        Task<IReadOnlyList<CompositionResult>> FindCompositionAsync(int count = 1);
        Task GenerateAllCompositionsForRoomAsync(RoomConfig room, CancellationToken cancellationToken = default);
        Task AddResults(int compositionId, CompositionResultsEntity[] compositions);

    }
}
