using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestiaryArenaCracker.Repository.Repositories
{
    public class CompositionRepository(IApplicationDbContext dbContext) : ICompositionRepository
    {
        public async Task<CompositionEntity> AddCompositionAsync(CompositionEntity entity)
        {
            dbContext.Compositions.Add(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task AddMonstersAsync(int compositionId, IEnumerable<CompositionMonstersEntity> monsters)
        {
            dbContext.CompositionMonsters.AddRange(monsters.Select(c =>
            {
                c.CompositionId = compositionId;
                return c;
            }));
            await dbContext.SaveChangesAsync();
        }

        public Task AddResults(int compositionId, CompositionResultsEntity[] results)
        {
            dbContext.CompositionResults.AddRange(results.Select(r =>
            {
                r.CompositionId = compositionId;
                return r;
            }));

            return dbContext.SaveChangesAsync();
        }

        public Task<bool> CompositionExistsAsync(string roomId, string hash)
        {
            return dbContext.Compositions
                .AsNoTracking()
                .AnyAsync(c => c.CompositionHash == hash && c.RoomId == roomId);
        }

        public Task<CompositionMonstersEntity[]> GetMonstersByCompositionIdAsync(int compositionId)
        {
            return dbContext.CompositionMonsters
                .AsNoTracking()
                .Where(m => m.CompositionId == compositionId)
                .ToArrayAsync();
        }

        public Task<CompositionEntity?> GetNextAvailableCompositionAsync(string roomId, int maxResults)
        {
            return GetNextAvailableCompositionAsync(roomId, maxResults, new HashSet<int>());
        }

        public Task<CompositionEntity?> GetNextAvailableCompositionAsync(string roomId, int maxResults, IReadOnlySet<int> excludedIds)
        {
            return dbContext.Compositions
                .AsNoTracking()
                .Where(c => c.RoomId == roomId && !excludedIds.Contains(c.Id))
                .GroupJoin(
                    dbContext.CompositionResults.AsNoTracking(),
                    c => c.Id,
                    r => r.CompositionId,
                    (c, results) => new { Composition = c, ResultCount = results.Count() })
                .Where(x => x.ResultCount < maxResults)
                .OrderBy(x => x.Composition.Id)
                .Select(x => x.Composition)
                .FirstOrDefaultAsync();
        }

        public Task<int> GetResultsCountAsync(int compositionId)
        {
            return dbContext.CompositionResults
                .AsNoTracking()
                .CountAsync(r => r.CompositionId == compositionId);
        }
    }
}
