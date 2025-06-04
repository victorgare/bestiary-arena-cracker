using BestiaryArenaCracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BestiaryArenaCracker.Repository.EntityConfiguration
{
    public class CompositionMonstersEntityConfiguration : IEntityTypeConfiguration<CompositionMonstersEntity>
    {
        public void Configure(EntityTypeBuilder<CompositionMonstersEntity> builder)
        {
            builder.ToTable("CompositionMonsters");
            builder.HasKey(cm => cm.Id);
        }
    }
}
