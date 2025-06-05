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
    public class CompositionService(IRoomConfigProvider roomConfigProvider, IApplicationDbContext dbContext) : ICompositionService
    {
        private const int MaxResultsPerComposition = 300;
        public async Task<CompositionResult?> FindCompositionAsync()
        {
            foreach (var room in roomConfigProvider.Rooms)
            {
                var composition = await GetNextOrGenerate(room);

                if (composition == null)
                {
                    continue;
                }

                // Get monsters for this composition
                var monsters = dbContext.CompositionMonsters
                    .Where(m => m.CompositionId == composition.Id)
                    .ToList();

                // Calculate remaining results
                var resultsCount = dbContext.CompositionResults
                    .Count(r => r.CompositionId == composition.Id);
                var remainingResults = MaxResultsPerComposition - resultsCount;

                // Build the result object
                var compositionResult = new CompositionResult
                {
                    RemainingRuns = MaxResultsPerComposition - resultsCount,
                    Composition = new Composition
                    {
                        Map = room.Name,
                        Board = [.. monsters.Select(m => new Board
                        {
                            Tile = m.TileLocation,
                            Monster = new Monster
                            {
                                Name = m.Name.ToLowerInvariant(),
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

                // You can serialize this result to JSON in your controller, or return as-is if using ASP.NET Core's automatic serialization
                return compositionResult;
            }

            return null!;
        }

        public async Task<CompositionEntity?> GetNextOrGenerate(RoomConfig room)
        {
            // 1. Try to find a composition for this room with < MaxResultsPerComposition results
            var composition = dbContext.Compositions
                .Where(c => c.RoomId == room.Id)
                .OrderBy(c => c.Id)
                .FirstOrDefault(c =>
                    dbContext.CompositionResults.Count(r => r.CompositionId == c.Id) < MaxResultsPerComposition);

            if (composition != null)
            {
                return composition;
            }

            // 2a. Generate all possible valid compositions (combinatorial logic)
            foreach (var team in GenerateValidTeams(room))
            {
                foreach (var positions in GenerateValidPositions(room, team.Count))
                {
                    foreach (var equippedTeam in GenerateEquipmentAndStats(team))
                    {
                        var hash = ComputeCompositionHash(room, equippedTeam, positions);

                        // Check if this composition already exists
                        if (dbContext.Compositions.Any(c => c.CompositionHash == hash && c.RoomId == room.Id))
                            continue;

                        // 3. Insert new composition and its monsters
                        var newComposition = new CompositionEntity
                        {
                            CompositionHash = hash,
                            RoomId = room.Id
                        };

                        dbContext.Compositions.Add(newComposition);
                        await dbContext.SaveChangesAsync();

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

                        dbContext.CompositionMonsters.AddRange(compositionMonsters);
                        await dbContext.SaveChangesAsync();

                        return newComposition;
                    }
                }
            }

            return null;
        }

        // Helper: Generate all valid teams (unique creatures, skip solo-useless)
        private IEnumerable<List<CompositionMonstersEntity>> GenerateValidTeams(RoomConfig room)
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
        private IEnumerable<List<int>> GenerateValidPositions(RoomConfig room, int teamSize)
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

                foreach (var eq in equipment)
                {
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
            // Example: hash room id + sorted creature+equipment+stat+position
            var parts = team
                .Select((c, i) => $"{c.Name}-{positions[i]}-{c.Equipment}-{c.EquipmentStat}")
                .OrderBy(s => s);
            var raw = $"{room.Id}:{string.Join("|", parts)}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(raw);
            return Convert.ToBase64String(SHA256.HashData(bytes));
        }

        // Helper: Get all combinations of a list (n choose k)
        private IEnumerable<List<T>> GetCombinations<T>(IEnumerable<T> list, int length)
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
