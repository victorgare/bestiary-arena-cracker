using BestiaryArenaCracker.ApplicationCore.Services.Composition;
using BestiaryArenaCracker.Domain;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Services
{
    public interface ICompositionService
    {
        Task<CompositionResult?> FindCompositionAsync();
    }
}
