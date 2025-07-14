using BestiaryArenaCracker.Domain.Entities;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories
{
    public interface ICompositionRepository
    {
        Task<CompositionEntity[]> GetNextAvailableCompositionsAsync(string roomId, int maxResults, IReadOnlySet<int> excludedIds, int take = 5);
        Task<bool> CompositionExistsAsync(string roomId, string hash);
        Task<CompositionEntity> AddCompositionAsync(CompositionEntity entity);
        Task AddMonstersAsync(int compositionId, IEnumerable<CompositionMonstersEntity> monsters);
        Task<CompositionEntity> AddCompositionWithMonstersAsync(CompositionEntity entity, IEnumerable<CompositionMonstersEntity> monsters);
        Task<CompositionMonstersEntity[]> GetMonstersByCompositionIdAsync(int compositionId);
        Task<int> GetResultsCountAsync(int compositionId);
        Task AddResults(int compositionId, CompositionResultsEntity[] results);
    }
}
