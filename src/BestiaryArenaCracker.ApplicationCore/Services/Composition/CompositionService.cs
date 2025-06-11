using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;
using BestiaryArenaCracker.Domain;
using BestiaryArenaCracker.Domain.Entities;
using BestiaryArenaCracker.Domain.Extensions;
using BestiaryArenaCracker.Domain.Room;
using System.Security.Cryptography;

namespace BestiaryArenaCracker.ApplicationCore.Services.Composition
{
    public class CompositionService(IRoomConfigProvider roomConfigProvider, ICompositionRepository compositionRepository) : ICompositionService
    {
        private const int MaxResultsPerComposition = 10;
        public async Task<CompositionResult?> FindCompositionAsync()
        {
            foreach (var room in roomConfigProvider.Rooms)
            {
                var composition = await compositionRepository.GetNextAvailableCompositionAsync(room.Id, MaxResultsPerComposition);

                if (composition == null)
                    continue;

                var monsters = await compositionRepository.GetMonstersByCompositionIdAsync(composition.Id);
                var resultsCount = await compositionRepository.GetResultsCountAsync(composition.Id);

                return new CompositionResult
                {
                    CompositionId = composition.Id,
                    RemainingRuns = MaxResultsPerComposition - resultsCount,
                    Composition = new Composition
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
            }

            return null!;
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

                        await compositionRepository.AddCompositionAsync(newComposition);

                        var compositionMonsters = new List<CompositionMonstersEntity>();
                        for (int i = 0; i < equippedTeam.Count; i++)
                        {
                            var member = equippedTeam[i];
                            var monster = new CompositionMonstersEntity
                            {
                                CompositionId = newComposition.Id,
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

                        await compositionRepository.AddMonstersAsync(newComposition.Id, compositionMonsters);

                        if (cancellationToken.IsCancellationRequested)
                            return;
                    }
                }
            }
        }

        // Helper: Generate all valid teams (unique creatures, skip solo-useless)
        private static IEnumerable<List<CompositionMonstersEntity>> GenerateValidTeams(RoomConfig room)
        {
            var allCreatures = Enum.GetValues<Creatures>()
                .Cast<Creatures>()
                .Where(c => !c.IsSoloUseless() || room.MaxTeamSize > 1)
                .ToList();

            for (int teamSize = 1; teamSize <= room.MaxTeamSize; teamSize++)
            {
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
            var equipment = Enum.GetValues<Equipments>().Cast<Equipments>();
            var stats = Enum.GetValues<EquipmentStat>().Cast<EquipmentStat>();

            IEnumerable<List<CompositionMonstersEntity>> Expand(int index, List<CompositionMonstersEntity> current)
            {
                if (index == team.Count)
                {
                    yield return current;
                    yield break;
                }

                // Try to parse the creature name to the enum
                var creatureName = team[index].Name;
                var canParse = Enum.TryParse<Creatures>(creatureName, out var creature);

                foreach (var eq in equipment)
                {
                    // Skip equipment if the creature should not have it
                    if (canParse && creature.SkipsEquipment(eq))
                        continue;

                    foreach (var stat in stats)
                    {
                        var next = new List<CompositionMonstersEntity>(current)
                {
                    new() {
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
                    }
                };

                        foreach (var result in Expand(index + 1, next))
                        {
                            yield return result;
                        }
                    }
                }
            }

            return Expand(0, []);
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


        public Task AddResults(int compositionId, CompositionResultsEntity[] compositions)
        {
            return compositionRepository.AddResults(compositionId, compositions);
        }

        public Task<Int128> CalculatePossibleCompositions(RoomConfig room)
        {
            var allCreatures = Enum.GetValues<Creatures>()
         .Cast<Creatures>()
         .Where(c => !c.IsSoloUseless() || room.MaxTeamSize > 1)
         .ToArray();

            var freeTiles = room.File.Data.GetFreeTiles();
            var allEquipments = Enum.GetValues<Equipments>().Cast<Equipments>().ToArray();
            var allStats = Enum.GetValues<EquipmentStat>().Cast<EquipmentStat>().ToArray();

            // Precompute valid equipment/stat count for each creature
            var validEquipStatCount = allCreatures
                .Select(creature =>
                    allEquipments.Count(eq => !creature.SkipsEquipment(eq)) * allStats.Length
                )
                .ToArray();

            Int128 total = 0;

            for (int teamSize = 1; teamSize <= room.MaxTeamSize; teamSize++)
            {
                var nCreatures = allCreatures.Length;
                var nTiles = freeTiles.Length;

                // Number of unique positions (freeTiles choose teamSize)
                var posComb = Combinations(nTiles, teamSize);

                // Sum over all unique teams (n choose k), product of validEquipStatCount for each team
                Int128 teamEquipCombSum = 0;

                foreach (var indices in GetIndexCombinations(nCreatures, teamSize))
                {
                    Int128 prod = 1;
                    foreach (var idx in indices)
                        prod *= validEquipStatCount[idx];
                    teamEquipCombSum += prod;
                }

                total += posComb * teamEquipCombSum;
            }

            return Task.FromResult(total);
        }

        // Helper: Get all index combinations (n choose k)
        private static IEnumerable<int[]> GetIndexCombinations(int n, int k)
        {
            int[] result = new int[k];
            Stack<(int i, int next)> stack = new();
            stack.Push((0, 0));
            while (stack.Count > 0)
            {
                var (i, next) = stack.Pop();
                if (i == k)
                {
                    yield return (int[])result.Clone();
                }
                else
                {
                    for (int j = n - 1; j >= next; j--)
                    {
                        result[i] = j;
                        stack.Push((i + 1, j + 1));
                    }
                }
            }
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

        private static Int128 Combinations(int n, int k)
        {
            if (k > n) return 0;
            if (k == 0 || k == n) return 1;
            long result = 1;
            for (int i = 1; i <= k; i++)
            {
                result = result * (n - (k - i)) / i;
            }
            return (int)result;
        }
    }
}
