using BestiaryArenaCracker.Domain.Room;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Providers
{
    public interface IRoomConfigProvider
    {
        IReadOnlyList<RoomConfig> Rooms { get; }
    }
}