using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.Domain.Room;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BestiaryArenaCracker.ApplicationCore.Providers
{
    public class RoomConfigProvider : IRoomConfigProvider
    {
        public IReadOnlyList<RoomConfig> Rooms { get; }
        public IReadOnlyList<RoomConfig> AllRooms { get; }
        public IReadOnlySet<string> BoostedRoomId { get; }

        // ignore this rooms, for generations purposes only
        private static readonly string[] IgnoredRoomsIds = ["rkswrs", "rkwht"];

        public RoomConfigProvider()
        {
            var basePath = AppContext.BaseDirectory;
            var jsonPath = Path.Combine(basePath, "data", "rooms_config.json");
            var json = System.IO.File.ReadAllText(jsonPath);

            var jsonConfig = JsonSerializer.Deserialize<List<RoomConfig>>(json, options: new()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            }) ?? [];

            Rooms = [.. jsonConfig.Where(c => !IgnoredRoomsIds.Contains(c.Id))];
            AllRooms = jsonConfig;

            // update this when boosted room reset
            BoostedRoomId = new HashSet<string> { "rkevgr" };
        }
    }
}