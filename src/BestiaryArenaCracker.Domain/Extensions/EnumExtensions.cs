using BestiaryArenaCracker.Domain.Attributes;
using System.ComponentModel;
using System.Reflection;

namespace BestiaryArenaCracker.Domain.Extensions
{
    public static class EnumExtensions
    {
        private static readonly Dictionary<Creatures, Equipments[]> _allowedEquipmentCache;

        static EnumExtensions()
        {
            _allowedEquipmentCache = Enum.GetValues<Creatures>()
                .ToDictionary(c => c, c =>
                {
                    var member = typeof(Creatures).GetMember(c.ToString()).FirstOrDefault();
                    var attr = member?.GetCustomAttribute<AllowedEquipmentsAttribute>();
                    return attr?.Equipments ?? Array.Empty<Equipments>();
                });
        }
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

        public static bool SkipEquipment(this Creatures creature, Equipments equipment)
        {
            var member = typeof(Creatures).GetMember(creature.ToString()).FirstOrDefault();
            if (member == null) return false;

            var attr = member.GetCustomAttribute<SkipEquipmentAttribute>();
            return attr != null && attr.EquipmentsToSkip.Contains(equipment);
        }

        public static bool AllowedEquipment(this Creatures creature, Equipments equipment)
        {
            var member = typeof(Creatures).GetMember(creature.ToString()).FirstOrDefault();
            if (member == null) return false;

            var attr = member.GetCustomAttribute<AllowedEquipmentsAttribute>();
            return attr != null && attr.Equipments.Contains(equipment);
        }

        public static Equipments[] GetAllowedEquipments(this Creatures creature)
        {
            if (_allowedEquipmentCache.TryGetValue(creature, out var values))
                return values;
            return Array.Empty<Equipments>();
        }
    }
}