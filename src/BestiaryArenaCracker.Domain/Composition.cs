using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BestiaryArenaCracker.Domain
{
    public class Composition
    {
        public BoardItem[] Board { get; set; } = [];

        private readonly JsonSerializerOptions options = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public string ComputeHash()
        {
            // Serialize the object
            string json = JsonSerializer.Serialize(this, options);

            // Compute SHA256 hash
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
            return Convert.ToHexString(hashBytes);
        }
    }
}
