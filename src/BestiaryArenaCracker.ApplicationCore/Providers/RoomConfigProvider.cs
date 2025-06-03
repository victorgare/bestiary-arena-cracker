using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.Domain.Room;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BestiaryArenaCracker.ApplicationCore.Providers
{
    public class RoomConfigProvider : IRoomConfigProvider
    {
        public IReadOnlyList<RoomConfig> Rooms { get; }

        public RoomConfigProvider()
        {
            var json = System.IO.File.ReadAllText("data/rooms_config.json");
            Rooms = JsonSerializer.Deserialize<List<RoomConfig>>(json, options: new()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            }) ?? [];
        }
    }
}