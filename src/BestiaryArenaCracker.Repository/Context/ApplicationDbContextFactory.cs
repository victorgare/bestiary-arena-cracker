using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BestiaryArenaCracker.Repository.Context
{
    /// <summary>
    /// this is for ef core tools only, it is used to create migrations and update the database
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            // Use your actual connection string here or load from configuration
            optionsBuilder.UseSqlServer("BestiaryArenaCracker");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
