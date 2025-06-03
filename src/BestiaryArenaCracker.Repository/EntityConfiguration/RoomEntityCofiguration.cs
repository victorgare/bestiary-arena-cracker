using BestiaryArenaCracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BestiaryArenaCracker.Repository.EntityConfiguration
{
    public class RoomEntityCofiguration : IEntityTypeConfiguration<IRoomEntity>
    {
        public void Configure(EntityTypeBuilder<IRoomEntity> builder)
        {
            builder.ToTable("Rooms");
            builder.HasKey(c => c.Id);
        }
    }
}
