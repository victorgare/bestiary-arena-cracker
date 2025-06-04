using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.Repository.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace BestiaryArenaCracker.Repository.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompositionEntityCofiguration).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
