using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;
using BestiaryArenaCracker.Domain;
using BestiaryArenaCracker.Domain.Composition;
using BestiaryArenaCracker.Domain.Constants;
using BestiaryArenaCracker.Domain.Entities;
using BestiaryArenaCracker.Domain.Extensions;
using BestiaryArenaCracker.Domain.Room;
using System;
using RedLockNet;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace BestiaryArenaCracker.ApplicationCore.Services.Composition
{
    public class CompositionService(
        IRoomConfigProvider roomConfigProvider,
        ICompositionRepository compositionRepository,
        IConnectionMultiplexer connectionMultiplexer,
        IDistributedLockFactory distributedLockFactory) : ICompositionService
    {
        private static readonly TimeSpan ReservationTtl = TimeSpan.FromMinutes(10);
        private static readonly ConcurrentDictionary<int, DateTime> _inUse = new();

        public async Task<IReadOnlyList<CompositionResult>> FindCompositionAsync(int count = 1)
        {
            var allRoomsExceptBoosted = roomConfigProvider.AllRooms.Where(c => !roomConfigProvider.BoostedRoomId.Contains(c.Id));
            var excludedIds = new HashSet<int>();
            var results = new List<CompositionResult>();

            // clean expired reservations and use them to skip DB lookups
            var now = DateTime.UtcNow;
            foreach (var kvp in _inUse.ToArray())
            {
                if (kvp.Value <= now)
                    _inUse.TryRemove(kvp.Key, out _);
            }

            foreach (var id in _inUse.Keys)
            {
                excludedIds.Add(id);
            }


            foreach (var room in allRoomsExceptBoosted)
            {
                while (results.Count < count)
                {
                    var compositions = await compositionRepository.GetNextAvailableCompositionsAsync(
                        room.Id,
                        ConfigurationConstants.DefaultMinimumCompositionRuns,
                        excludedIds,
                        count);

                    if (compositions.Length == 0)
                        break;

                    foreach (var composition in compositions)
                    {
                        var reserved = await distributedLockFactory.CreateLockAsync($"composition:{composition.Id}:reserved", ReservationTtl);
                        if (!reserved.IsAcquired)
                        {
                            excludedIds.Add(composition.Id);
                            continue;
                        }

                        var monsters = await compositionRepository.GetMonstersByCompositionIdAsync(composition.Id);
                        var resultsCount = await compositionRepository.GetResultsCountAsync(composition.Id);

                        var result = new CompositionResult
                        {
                            CompositionId = composition.Id,
                            RemainingRuns = ConfigurationConstants.DefaultMinimumCompositionRuns - resultsCount,
                            Composition = new Domain.Composition.Composition
                            {
                                Map = room.Name,
                                Board = [.. monsters.Select(m => new Board
                            {
                                Tile = m.TileLocation,
                                Monster = new Monster
                                {
                                    Name = m.Name.ToDisplayName(),
                                    Hp = m.Hitpoints,
                                    Ad = m.Attack,
                                    Ap = m.AbilityPower,
                                    Armor = m.Armor,
                                    MagicResist = m.MagicResistance,
                                    Level = m.Level
                                },
                                Equipment = new Equipment
                                {
                                    Name = m.Equipment.GetDescription(),
                                    Stat = m.EquipmentStat.GetDescription(),
                                    Tier = m.EquipmentTier
                                }
                            })]
                            }
                        };

                        results.Add(result);
                        _inUse[composition.Id] = DateTime.UtcNow.Add(ReservationTtl);
                    }

                    count -= results.Count;
                }

                if (results.Count >= count)
                    break;
            }

            return results;
        }

        // Public method for the worker to generate all compositions for a room
        public async Task GenerateAllCompositionsForRoomAsync(RoomConfig room, CancellationToken cancellationToken = default)
        {
            foreach (var team in GenerateValidTeams(room))
            {
                foreach (var positions in GenerateValidPositions(room, team.Count))
                {
                    foreach (var equippedTeam in GenerateEquipmentAndStats(team))
                    {
                        var hash = ComputeCompositionHash(room, equippedTeam, positions);

                        if (await compositionRepository.CompositionExistsAsync(room.Id, hash))
                            continue;

                        var newComposition = new CompositionEntity
                        {
                            CompositionHash = hash,
                            RoomId = room.Id
                        };

                        var compositionMonsters = new List<CompositionMonstersEntity>();
                        for (int i = 0; i < equippedTeam.Count; i++)
                        {
                            var member = equippedTeam[i];
                            var monster = new CompositionMonstersEntity
                            {
                                Composition = newComposition,
                                Name = member.Name,
                                Hitpoints = member.Hitpoints,
                                Attack = member.Attack,
                                AbilityPower = member.AbilityPower,
                                Armor = member.Armor,
                                MagicResistance = member.MagicResistance,
                                Level = member.Level,
                                TileLocation = positions[i],
                                Equipment = member.Equipment,
                                EquipmentStat = member.EquipmentStat,
                                EquipmentTier = member.EquipmentTier
                            };

                            compositionMonsters.Add(monster);
                        }

                        await compositionRepository.AddCompositionWithMonstersAsync(newComposition, compositionMonsters);

                        if (cancellationToken.IsCancellationRequested)
                            return;
                    }
                }
            }
        }

        // Helper: Generate all valid teams (unique creatures, skip solo-useless)
        private static IEnumerable<List<CompositionMonstersEntity>> GenerateValidTeams(RoomConfig room)
        {
            for (int teamSize = 1; teamSize <= room.MaxTeamSize; teamSize++)
            {
                var allCreatures = Enum.GetValues<Creatures>()
                 .Cast<Creatures>()
                 .Where(c => teamSize != 1 || !c.IsSoloUseless())
                 .ToList();

                foreach (var team in GetCombinations(allCreatures, teamSize))
                {
                    yield return team.Select(c => new CompositionMonstersEntity
                    {
                        Name = c.ToString()
                    }).ToList();
                }
            }
        }

        // Helper: Generate all valid positions for a team
        private static IEnumerable<List<int>> GenerateValidPositions(RoomConfig room, int teamSize)
        {
            var freeTiles = room.File.Data.GetFreeTiles();
            foreach (var positions in GetCombinations(freeTiles, teamSize))
            {
                yield return positions.ToList();
            }
        }

        // Helper: Generate all equipment/stat combos for a team
        private static IEnumerable<List<CompositionMonstersEntity>> GenerateEquipmentAndStats(List<CompositionMonstersEntity> team)
        {
            var allEquipments = Enum.GetValues<Equipments>().Cast<Equipments>().ToArray();
            var stats = Enum.GetValues<EquipmentStat>().Cast<EquipmentStat>().ToArray();

            // Cache equipments for each creature to avoid repeated reflection and allocations
            var allowedPerCreature = new Equipments[team.Count][];
            for (var i = 0; i < team.Count; i++)
            {
                var creatureName = team[i].Name;
                var canParse = Enum.TryParse<Creatures>(creatureName, out var creature);
                var allowedEquipments = canParse ? creature.GetAllowedEquipments() : Array.Empty<Equipments>();
                allowedPerCreature[i] = allowedEquipments.Length > 0
                    ? allowedEquipments
                    : canParse
                        ? allEquipments.Where(eq => !creature.SkipEquipment(eq)).ToArray()
                        : allEquipments;
            }

            var current = new CompositionMonstersEntity[team.Count];

            IEnumerable<List<CompositionMonstersEntity>> Expand(int index)
            {
                if (index == team.Count)
                {
                    // materialize a copy for the caller
                    yield return current.Select(c => new CompositionMonstersEntity
                    {
                        Name = c.Name,
                        Equipment = c.Equipment,
                        EquipmentStat = c.EquipmentStat,
                        Hitpoints = c.Hitpoints,
                        Attack = c.Attack,
                        AbilityPower = c.AbilityPower,
                        Armor = c.Armor,
                        MagicResistance = c.MagicResistance,
                        Level = c.Level,
                        EquipmentTier = c.EquipmentTier
                    }).ToList();
                    yield break;
                }

                foreach (var eq in allowedPerCreature[index])
                {
                    foreach (var stat in stats)
                    {
                        current[index] = new CompositionMonstersEntity
                        {
                            Name = team[index].Name,
                            Equipment = eq,
                            EquipmentStat = stat,
                            Hitpoints = team[index].Hitpoints,
                            Attack = team[index].Attack,
                            AbilityPower = team[index].AbilityPower,
                            Armor = team[index].Armor,
                            MagicResistance = team[index].MagicResistance,
                            Level = team[index].Level,
                            EquipmentTier = team[index].EquipmentTier
                        };

                        foreach (var result in Expand(index + 1))
                        {
                            yield return result;
                        }
                    }
                }
            }

            return Expand(0);
        }

        // Helper: Compute a unique hash for a composition
        private static string ComputeCompositionHash(RoomConfig room, List<CompositionMonstersEntity> team, List<int> positions)
        {
            // Example: hash room id + sorted creature+creature stats+equipment+stat+position
            var parts = team
                .Select((c, i) => $"{c.Name}-{positions[i]}-{c.Hitpoints}-{c.Attack}-{c.AbilityPower}-{c.Armor}-{c.MagicResistance}-{c.Level}-{c.Equipment}-{c.EquipmentStat}-{c.EquipmentTier}")
                .OrderBy(s => s);
            var raw = $"{room.Id}:{string.Join("|", parts)}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(raw);
            return Convert.ToBase64String(SHA256.HashData(bytes));
        }


        public async Task AddResults(int compositionId, CompositionResultsEntity[] compositions)
        {
            await compositionRepository.AddResults(compositionId, compositions);
            var db = connectionMultiplexer.GetDatabase();
            await db.KeyDeleteAsync($"composition:{compositionId}:reserved");
            _inUse.TryRemove(compositionId, out _);
        }

        // Helper: Get all combinations of a list (n choose k)
        private static IEnumerable<List<T>> GetCombinations<T>(IEnumerable<T> list, int length)
        {
            if (length == 0)
            {
                yield return new List<T>();
            }
            else
            {
                int i = 0;
                foreach (var item in list)
                {
                    var remaining = list.Skip(i + 1);
                    foreach (var combo in GetCombinations(remaining, length - 1))
                    {
                        combo.Insert(0, item);
                        yield return combo;
                    }
                    i++;
                }
            }
        }

    }
}
