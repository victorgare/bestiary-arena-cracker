using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.Domain.Room;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BestiaryArenaCracker.ApplicationCore.Providers
{
    public class RoomConfigProvider : IRoomConfigProvider
    {
        public IReadOnlyList<RoomConfig> Rooms { get; }

        public RoomConfigProvider()
        {
            var basePath = AppContext.BaseDirectory;
            var jsonPath = Path.Combine(basePath, "data", "rooms_config.json");
            var json = System.IO.File.ReadAllText(jsonPath);
            Rooms = JsonSerializer.Deserialize<List<RoomConfig>>(json, options: new()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            }) ?? [];
        }
    }
}