using BestiaryArenaCracker.Domain.Room;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Providers
{
    public interface IRoomConfigProvider
    {
        // rooms filtered by IgnoredRoomsIds, for generations purposes only
        IReadOnlyList<RoomConfig> Rooms { get; }

        // list of all rooms, including those ignored by IgnoredRoomsIds
        IReadOnlyList<RoomConfig> AllRooms { get; }

    }
}