using BestiaryArenaCracker.Domain;
using BestiaryArenaCracker.Domain.Composition;
using BestiaryArenaCracker.Domain.Entities;
using BestiaryArenaCracker.Domain.Extensions;
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
            builder
                .Property(c => c.Equipment)
                .HasConversion<string>();

            builder.Property(c => c.EquipmentStat)
                .HasConversion(
                    v => v.GetDescription(), // To database
                    v => Enum.GetValues<EquipmentStat>().First(e => e.GetDescription() == v) // From database
                );

            builder
                .HasOne(cm => cm.Composition)
                .WithMany()
                .HasForeignKey(cm => cm.CompositionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
