namespace BestiaryArenaCracker.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AllowedEquipmentsAttribute : Attribute
    {
        public Equipments[] Equipments { get; }
        public AllowedEquipmentsAttribute(params Equipments[] equipments)
        {
            Equipments = equipments;
        }
    }
}
