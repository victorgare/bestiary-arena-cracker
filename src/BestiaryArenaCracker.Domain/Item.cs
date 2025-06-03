using BestiaryArenaCracker.Domain.Extensions;

namespace BestiaryArenaCracker.Domain
{
    public class Item(Equipments equipment, EquipmentStat equipmentStat)
    {
        public Equipments Equipment { get; private set; } = equipment;
        public EquipmentStat EquipmentStat { get; private set; } = equipmentStat;

        public string Name => Equipment.ToString();
        public string Stat => EquipmentStat.GetDescription();
        public int Tier { get; set; } = 5;
    }
}
