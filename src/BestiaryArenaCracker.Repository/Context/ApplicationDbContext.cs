using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.Domain.Entities;
using BestiaryArenaCracker.Repository.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace BestiaryArenaCracker.Repository.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
    {
        public DbSet<CompositionEntity> Compositions { get; set; }
        public DbSet<CompositionMonstersEntity> CompositionMonsters { get; set; }
        public DbSet<CompositionResultsEntity> CompositionResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompositionEntityCofiguration).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
