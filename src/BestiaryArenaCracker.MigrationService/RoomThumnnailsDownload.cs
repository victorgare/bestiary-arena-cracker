using System.Diagnostics;
using System.Text.Json;

namespace BestiaryArenaCracker.MigrationService
{
    public class RoomThumnnailsDownload : BackgroundService
    {
        public const string ActivitySourceName = "Migrations";
        private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var activity = s_activitySource.StartActivity("Downloading thumbnails", ActivityKind.Client);

            // Path to your rooms_config.json file
            var basePath = AppContext.BaseDirectory;
            var solutionRoot = Path.GetFullPath(Path.Combine(basePath, "../../../../.."));
            var jsonPath = Path.Combine(solutionRoot, "data", "rooms_config.json");
            var outputDir = Path.Combine(solutionRoot, "src/BestiaryArenaCracker.UI/public/rooms");

            // Ensure output directory exists
            Directory.CreateDirectory(outputDir);

            // Read and parse rooms_config.json
            var roomsJson = await File.ReadAllTextAsync(jsonPath, stoppingToken);
            var rooms = JsonSerializer.Deserialize<List<RoomConfig>>(roomsJson);

            using var httpClient = new HttpClient();
            foreach (var room in rooms)
            {
                var roomId = room.id;
                var imageUrl = $"https://bestiaryarena.com/assets/room-thumbnails/{roomId}.png";
                var filePath = Path.Combine(outputDir, $"{roomId}.png");

                // Only download if the file does not exist
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"{roomId}.png already exists, skipping download.");
                    continue;
                }

                try
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                    await File.WriteAllBytesAsync(filePath, imageBytes, stoppingToken);
                    Console.WriteLine($"Downloaded {roomId}.png");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download {roomId}: {ex.Message}");
                }
            }

        }
    }
}

// RoomConfig class for deserialization
public class RoomConfig
{
    public string id { get; set; }
}