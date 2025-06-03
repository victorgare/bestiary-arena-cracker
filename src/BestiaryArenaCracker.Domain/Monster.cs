using System.Text.Json.Serialization;

namespace BestiaryArenaCracker.Domain
{
    public class Monster(Creatures creature)
    {
        public Creatures Creature { get; } = creature;

        public string Name => Creature.ToString();

        [JsonPropertyName("hp")]
        public int Hitpoints { get; set; } = 20;

        [JsonPropertyName("ad")]
        public int Attack { get; set; } = 20;

        [JsonPropertyName("ap")]
        public int AbilityPower { get; set; } = 20;
        public int Armor { get; set; } = 20;

        [JsonPropertyName("magicResist")]
        public int MagicResistance { get; set; } = 20;
        public int Level { get; set; } = 50;

    }
}
