using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;
using BestiaryArenaCracker.Domain;
using BestiaryArenaCracker.Domain.Composition;
using BestiaryArenaCracker.Domain.Constants;
using BestiaryArenaCracker.Domain.Entities;
using BestiaryArenaCracker.Domain.Extensions;
using BestiaryArenaCracker.Domain.Room;
using System.Security.Cryptography;

namespace BestiaryArenaCracker.ApplicationCore.Services.Composition
{
    public class CompositionService(
        IRoomConfigProvider roomConfigProvider,
        ICompositionRepository compositionRepository) : ICompositionService
    {
        private static readonly TimedResourceLock<int> _compositionLock = new(TimeSpan.FromSeconds(10));

        public async Task<IReadOnlyList<CompositionResult>> FindCompositionAsync(int count = 1)
        {
            var allRoomsExceptBoosted = roomConfigProvider.AllRooms.Where(c => !roomConfigProvider.BoostedRoomId.Contains(c.Id));
            var results = new List<CompositionResult>();

            foreach (var room in allRoomsExceptBoosted)
            {
                while (results.Count < count)
                {
                    var compositions = await compositionRepository.GetNextAvailableCompositionsAsync(
                        room.Id,
                        ConfigurationConstants.DefaultMinimumCompositionRuns,
                        _compositionLock.ReservedValues,
                        count);

                    if (compositions.Length == 0)
                        break;

                    foreach (var composition in compositions)
                    {
                        if (!await _compositionLock.TryAcquireAsync(composition.Id))
                        {
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
                    }
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
                    var list = new List<CompositionMonstersEntity>(teamSize);
                    foreach (var c in team)
                    {
                        list.Add(new CompositionMonstersEntity { Name = c.ToString() });
                    }
                    yield return list;
                }
            }
        }

        // Helper: Generate all valid positions for a team
        private static IEnumerable<List<int>> GenerateValidPositions(RoomConfig room, int teamSize)
        {
            var freeTiles = room.File.Data.GetFreeTiles();
            foreach (var positions in GetCombinations(freeTiles, teamSize))
            {
                var list = new List<int>(teamSize);
                list.AddRange(positions);
                yield return list;
            }
        }

        // Helper: Generate all equipment/stat combos for a team
        private static IEnumerable<List<CompositionMonstersEntity>> GenerateEquipmentAndStats(List<CompositionMonstersEntity> team)
        {
            var allEquipments = Enum.GetValues<Equipments>();
            var stats = Enum.GetValues<EquipmentStat>();

            var allowedPerCreature = new Equipments[team.Count][];
            for (var i = 0; i < team.Count; i++)
            {
                if (Enum.TryParse<Creatures>(team[i].Name, out var creature))
                {
                    var allowed = creature.GetAllowedEquipments();
                    allowedPerCreature[i] = allowed.Length > 0
                        ? allowed
                        : allEquipments.Where(eq => !creature.SkipEquipment(eq)).ToArray();
                }
                else
                {
                    allowedPerCreature[i] = allEquipments.ToArray();
                }
            }

            var current = new CompositionMonstersEntity[team.Count];
            for (int i = 0; i < team.Count; i++)
            {
                current[i] = new CompositionMonstersEntity
                {
                    Name = team[i].Name,
                    Hitpoints = team[i].Hitpoints,
                    Attack = team[i].Attack,
                    AbilityPower = team[i].AbilityPower,
                    Armor = team[i].Armor,
                    MagicResistance = team[i].MagicResistance,
                    Level = team[i].Level,
                    EquipmentTier = team[i].EquipmentTier
                };
            }

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
                        current[index].Equipment = eq;
                        current[index].EquipmentStat = stat;

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
            _compositionLock.Release(compositionId);
        }

        // Helper: Get all combinations of a list (n choose k)
        private static IEnumerable<List<T>> GetCombinations<T>(IEnumerable<T> list, int length)
        {
            var items = list.ToArray();
            if (length == 0 || length > items.Length)
                yield break;

            var indices = Enumerable.Range(0, length).ToArray();
            var result = new T[length];

            while (true)
            {
                for (int i = 0; i < length; i++)
                    result[i] = items[indices[i]];
                yield return new List<T>(result);

                int t = length - 1;
                while (t >= 0 && indices[t] == items.Length - length + t)
                    t--;
                if (t < 0)
                    break;

                indices[t]++;
                for (int i = t + 1; i < length; i++)
                    indices[i] = indices[i - 1] + 1;
            }
        }

    }
}
