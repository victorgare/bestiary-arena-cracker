using BestiaryArenaCracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BestiaryArenaCracker.Repository.EntityConfiguration
{
    public class CompositionrResultsEntityConfiguration : IEntityTypeConfiguration<CompositionEntity>
    {
        public void Configure(EntityTypeBuilder<CompositionEntity> builder)
        {
            builder.ToTable("CompositionResult");
            builder.HasKey(c => c.Id);
        }
    }
}
