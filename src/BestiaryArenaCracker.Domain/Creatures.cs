using BestiaryArenaCracker.Domain.Attributes;

namespace BestiaryArenaCracker.Domain
{
    public enum Creatures
    {
        [SoloUseless]
        Banshee,

        [AllowedEquipments(
            Equipments.DwarvenLegs,
            Equipments.MedusaShield,
            Equipments.IceRapier)]
        Bear,

        [AllowedEquipments(Equipments.GlassOfGoo)]
        BogRaider,

        [SkipEquipment(Equipments.RubberCap)]
        Bug,

        [AllowedEquipments(Equipments.BootsOfHaste)]
        Cyclops,

        [SoloUseless]
        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.SteelBoots)]
        Deer,

        [AllowedEquipments(
            Equipments.BloodyEdge,
            Equipments.DwarvenHelmet,
            Equipments.FireAxe,
            Equipments.SkullcrackerArmor)]
        DemonSkeleton,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.DwarvenHelmet,
            Equipments.IceRapier,
            Equipments.WandOfDecay)]
        Dragon,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.DwarvenHelmet,
            Equipments.WandOfDecay)]
        DragonLord,

        [SoloUseless]
        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.DwarvenHelmet,
            Equipments.SpringsproutRod)]
        Druid,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.BonelordHelmet)]
        Dwarf,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.GiantSword)]
        DwarfGeomancer,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.DwarvenHelmet,
            Equipments.Epee,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        DwarfGuard,

        [AllowedEquipments(
            Equipments.ChainBolter,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        DwarfSoldier,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.ChainBolter,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        Elf,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.DwarvenHelmet,
            Equipments.FireAxe,
            Equipments.WandOfDecay)]
        ElfArcanist,

        [AllowedEquipments(
            Equipments.BootsOfHaste,
            Equipments.ChainBolter,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        ElfScout,

        [AllowedEquipments(
            Equipments.IceRapier,
            Equipments.BlueRobe,
            Equipments.WandOfDecay)]
        FireDevil,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.DwarvenHelmet,
            Equipments.IceRapier,
            Equipments.WandOfDecay)]
        FireElemental,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.BootsOfHaste,
            Equipments.IceRapier)]
        Firestarter,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.BonelordHelmet,
            Equipments.FireAxe)]
        FrostTroll,

        [SoloUseless]
        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.RubberCap,
            Equipments.SkullcrackerArmor)]
        Ghost,

        [AllowedEquipments(
            Equipments.DwarvenHelmet,
            Equipments.MedusaShield)]
        Ghoul,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.ChainBolter,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        Goblin,

        [AllowedEquipments(
            Equipments.BootsOfHaste,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        GoblinAssassin,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        GoblinScavenger,

        [AllowedEquipments(
            Equipments.GiantSword)]
        Knight,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.DwarvenHelmet,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        Minotaur,

        [AllowedEquipments(
            Equipments.ChainBolter,
            Equipments.FireAxe,
            Equipments.IceRapier,
            Equipments.Ratana,
            Equipments.SkullHelmet)]
        MinotaurArcher,

        [AllowedEquipments(
            Equipments.BloodyEdge,
            Equipments.DwarvenHelmet,
            Equipments.FireAxe,
            Equipments.MedusaShield,
            Equipments.VampireShield)]
        MinotaurGuard,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.DwarvenHelmet)]
        MinotaurMage,

        [AllowedEquipments(
            Equipments.DwarvenHelmet,
            Equipments.MedusaShield,
            Equipments.RoyalScaleRobe)]
        Monk,

        [AllowedEquipments(
            Equipments.CranialBasher,
            Equipments.IceRapier)]
        Mummy,

        [AllowedEquipments(Equipments.Epee)]
        Nightstalker,

        [AllowedEquipments(
            Equipments.BloodyEdge,
            Equipments.DwarvenHelmet,
            Equipments.FireAxe,
            Equipments.IceRapier,
            Equipments.Ratana,
            Equipments.SkullHelmet)]
        OrcBerserker,

        [AllowedEquipments(
            Equipments.BloodyEdge,
            Equipments.BlueRobe,
            Equipments.DwarvenHelmet,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        OrcLeader,

        [AllowedEquipments(
            Equipments.CranialBasher,
            Equipments.DwarvenHelmet,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        OrcRider,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.BonelordHelmet,
            Equipments.DwarvenHelmet,
            Equipments.Epee,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        OrcShaman,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.BootsOfHaste,
            Equipments.DwarvenHelmet,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        OrcSpearman,

        [AllowedEquipments(
            Equipments.BloodyEdge,
            Equipments.BlueRobe,
            Equipments.DwarvenHelmet,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        OrcWarlord,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.FireAxe,
            Equipments.GlassOfGoo,
            Equipments.IceRapier)]
        PoisonSpider,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.DwarvenHelmet,
            Equipments.GlacialRod,
            Equipments.SteelBoots)]
        PolarBear,

        [AllowedEquipments(
            Equipments.BloodyEdge,
            Equipments.DwarvenHelmet,
            Equipments.FireAxe,
            Equipments.IceRapier,
            Equipments.MedusaShield,
            Equipments.Ratana)]
        Rat,

        [AllowedEquipments(
            Equipments.BootsOfHaste,
            Equipments.DwarvenHelmet,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        Rorc,

        [AllowedEquipments(
            Equipments.DwarvenHelmet,
            Equipments.IceRapier,
            Equipments.MedusaShield,
            Equipments.RoyalScaleRobe)]
        Rotworm,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.FireAxe,
            Equipments.GlassOfGoo,
            Equipments.IceRapier,
            Equipments.SkullcrackerArmor)]
        Scorpion,

        [SoloUseless]
        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.SkullcrackerArmor,
            Equipments.SpringsproutRod)]
        Sheep,

        [AllowedEquipments(
            Equipments.FireAxe,
            Equipments.IceRapier,
            Equipments.SkullcrackerArmor)]
        Skeleton,


        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.BonelordHelmet,
            Equipments.FireAxe,
            Equipments.GlassOfGoo,
            Equipments.IceRapier,
            Equipments.SkullcrackerArmor)]
        Slime,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.FireAxe,
            Equipments.GlassOfGoo)]
        Snake,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.FireAxe,
            Equipments.GlassOfGoo)]
        Spider,

        [AllowedEquipments(
            Equipments.DwarvenHelmet,
            Equipments.EctoplasmicShield,
            Equipments.FireAxe,
            Equipments.IceRapier,
            Equipments.MedusaShield)]
        Stalker,

        [AllowedEquipments(
            Equipments.DwarvenHelmet,
            Equipments.SkullcrackerArmor)]
        Tortoise,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.BonelordHelmet)]
        Troll,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.BonelordHelmet,
            Equipments.IceRapier,
            Equipments.SkullcrackerArmor)]
        Warlock,

        [AllowedEquipments(
            Equipments.BearSkin,
            Equipments.FireAxe,
            Equipments.GlassOfGoo,
            Equipments.IceRapier)]
        Wasp,

        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.FireAxe,
            Equipments.IceRapier,
            Equipments.MedusaShield,
            Equipments.VampireShield)]
        WaterElemental,

        [AllowedEquipments(
            Equipments.ChainBolter,
            Equipments.Epee,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        WinterWolf,

        [AllowedEquipments(
            Equipments.ChainBolter,
            Equipments.Epee,
            Equipments.FireAxe,
            Equipments.IceRapier)]
        Wolf,
        [AllowedEquipments(
            Equipments.BlueRobe,
            Equipments.FireAxe,
            Equipments.IceRapier,
            Equipments.VampireShield)]
        Wyvern
    }
}
