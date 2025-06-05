namespace BestiaryArenaCracker.Domain.Entities
{
    public class CompositionMonstersEntity : IEntity
    {
        public int CompositionId { get; set; }
        public string Name { get; set; } = null!;
        public int Hitpoints { get; set; } = 20;
        public int Attack { get; set; } = 20;
        public int AbilityPower { get; set; } = 20;
        public int Armor { get; set; } = 20;
        public int MagicResistance { get; set; } = 20;
        public int Level { get; set; } = 20;
        public int TileLocation { get; set; }
        public Equipments Equipment { get; set; }
        public EquipmentStat EquipmentStat { get; set; }
        public int EquipmentTier { get; set; } = 5;
    }
}
