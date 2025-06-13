using BestiaryArenaCracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BestiaryArenaCracker.Repository.EntityConfiguration
{
    public class CompositionEntityCofiguration : IEntityTypeConfiguration<CompositionEntity>
    {
        public void Configure(EntityTypeBuilder<CompositionEntity> builder)
        {
            builder.ToTable("Compositions");
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.CompositionHash).IsUnique();
            builder.HasIndex(c => c.RoomId);
        }
    }
}
