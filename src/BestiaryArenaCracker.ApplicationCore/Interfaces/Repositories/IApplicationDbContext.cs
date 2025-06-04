using BestiaryArenaCracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories
{
    public interface IApplicationDbContext
    {
        DbSet<CompositionEntity> Compositions { get; set; }
        DbSet<CompositionMonstersEntity> CompositionMonsters { get; set; }
        DbSet<CompositionResultsEntity> CompositionResults { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
