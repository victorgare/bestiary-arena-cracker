namespace BestiaryArenaCracker.Domain.Entities
{
    public class CompositionMonstersEntity : IEntity
    {
        public int CompositionId { get; set; }
        public string Name { get; set; } = null!;
        public int Hitpoints { get; set; }
        public int Attack { get; set; }
        public int AbilityPower { get; set; }
        public int Armor { get; set; }
        public int MagicResistance { get; set; }
        public int Level { get; set; }
        public int TileLocation { get; set; }
        public Equipments Equipment { get; set; }
        public EquipmentStat EquipmentStat { get; set; }
        public int EquipmentTier { get; set; }
    }
}
