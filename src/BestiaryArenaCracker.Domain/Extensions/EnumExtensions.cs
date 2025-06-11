using BestiaryArenaCracker.Domain.Attributes;
using System.ComponentModel;
using System.Reflection;

namespace BestiaryArenaCracker.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
        {
            var type = value.GetType();
            var memInfo = type.GetMember(value.ToString());
            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return value.ToString();
        }

        public static bool IsSoloUseless<TEnum>(this TEnum value) where TEnum : Enum
        {
            var type = value.GetType();
            var memInfo = type.GetMember(value.ToString());
            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(SoloUselessAttribute), false);
                return attrs.Length > 0;
            }
            return false;
        }

        public static bool SkipsEquipment(this Creatures creature, Equipments equipment)
        {
            var member = typeof(Creatures).GetMember(creature.ToString()).FirstOrDefault();
            if (member == null) return false;

            var attr = member.GetCustomAttribute<SkipEquipmentAttribute>();
            return attr != null && attr.EquipmentsToSkip.Contains(equipment);
        }
    }
}