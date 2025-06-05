using BestiaryArenaCracker.ApplicationCore.Services;
using BestiaryArenaCracker.Domain;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Services
{
    public interface ICompositionService
    {
        Task<CompositionResult?> FindCompositionAsync();
    }
}
