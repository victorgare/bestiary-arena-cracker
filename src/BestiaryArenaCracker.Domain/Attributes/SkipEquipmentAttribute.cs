namespace BestiaryArenaCracker.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SkipEquipmentAttribute : Attribute
    {
        public Equipments[] EquipmentsToSkip { get; }
        public SkipEquipmentAttribute(params Equipments[] equipmentsToSkip)
        {
            EquipmentsToSkip = equipmentsToSkip;
        }
    }
}
