using BestiaryArenaCracker.Domain.Composition;
using BestiaryArenaCracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BestiaryArenaCracker.Repository.EntityConfiguration
{
    public class CompositionResultsEntityConfiguration : IEntityTypeConfiguration<CompositionResultsEntity>
    {
        public void Configure(EntityTypeBuilder<CompositionResultsEntity> builder)
        {
            builder.ToTable("CompositionResults");
            builder.HasKey(c => c.Id);

            builder
                .HasOne<CompositionEntity>()
                .WithMany()
                .HasForeignKey(x => x.CompositionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
