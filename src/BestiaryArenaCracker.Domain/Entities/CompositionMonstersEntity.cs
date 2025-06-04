namespace BestiaryArenaCracker.Domain.Entities
{
    public class CompositionMonstersEntity : IEntity
    {
        public required int CompositionId { get; set; }
        public required string Name { get; set; }
        public required int Hitpoints { get; set; }
        public required int Attack { get; set; }
        public required int AbilityPower { get; set; }
        public required int Armor { get; set; }
        public required int MagicResistance { get; set; }
        public required int Level { get; set; }
        public required int TileLocation { get; set; }
    }
}
