using BestiaryArenaCracker.Domain.Entities;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories
{
    public interface ICompositionRepository
    {
        Task<CompositionEntity?> GetNextAvailableCompositionAsync(string roomId, int maxResults);
        Task<bool> CompositionExistsAsync(string roomId, string hash);
        Task<CompositionEntity> AddCompositionAsync(CompositionEntity entity);
        Task AddMonstersAsync(int compositionId, IEnumerable<CompositionMonstersEntity> monsters);
        Task<CompositionMonstersEntity[]> GetMonstersByCompositionIdAsync(int compositionId);
        Task<int> GetResultsCountAsync(int compositionId);
    }
}
