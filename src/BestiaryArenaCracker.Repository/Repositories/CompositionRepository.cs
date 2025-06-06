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

        public Task<bool> CompositionExistsAsync(string roomId, string hash)
        {
            return dbContext.Compositions.AnyAsync(c => c.CompositionHash == hash && c.RoomId == roomId);
        }

        public Task<CompositionMonstersEntity[]> GetMonstersByCompositionIdAsync(int compositionId)
        {
            return dbContext.CompositionMonsters.Where(m => m.CompositionId == compositionId).ToArrayAsync();
        }

        public Task<CompositionEntity?> GetNextAvailableCompositionAsync(string roomId, int maxResults)
        {
            return dbContext.Compositions
                .Where(c => c.RoomId == roomId)
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync(c =>
                    dbContext.CompositionResults.Count(r => r.CompositionId == c.Id) < maxResults);
        }

        public Task<int> GetResultsCountAsync(int compositionId)
        {
            return dbContext.CompositionResults.CountAsync(r => r.CompositionId == compositionId);
        }
    }
}
