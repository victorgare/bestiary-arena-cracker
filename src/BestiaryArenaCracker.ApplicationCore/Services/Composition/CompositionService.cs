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
    public class CompositionService(IRoomConfigProvider roomConfigProvider, ICompositionRepository compositionRepository) : ICompositionService
    {
        public async Task<CompositionResult?> FindCompositionAsync()
        {
            var allRoomsExceptBoosted = roomConfigProvider.AllRooms.Where(c => !roomConfigProvider.BoostedRoomId.Contains(c.Id));

            foreach (var room in allRoomsExceptBoosted)
            {
                var composition = await compositionRepository.GetNextAvailableCompositionAsync(room.Id, ConfigurationConstants.DefaultMinimumCompositionRuns);

                if (composition == null)
                    continue;

                var monsters = await compositionRepository.GetMonstersByCompositionIdAsync(composition.Id);
                var resultsCount = await compositionRepository.GetResultsCountAsync(composition.Id);

                return new CompositionResult
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

                        if (cancellationToken.IsCancellationRequested)
                            return;

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
            var allEquipments = Enum.GetValues<Equipments>().Cast<Equipments>();
            var stats = Enum.GetValues<EquipmentStat>().Cast<EquipmentStat>();

            IEnumerable<List<CompositionMonstersEntity>> Expand(int index, List<CompositionMonstersEntity> current)
            {
                if (index == team.Count)
                {
                    yield return current;
                    yield break;
                }

                var creatureName = team[index].Name;
                var canParse = Enum.TryParse<Creatures>(creatureName, out var creature);

                // Se houver AllowedEquipments, use apenas eles. Senão, use todos exceto os do SkipEquipment.
                var allowedEquipments = canParse ? creature.GetAllowedEquipments() : [];
                var equipmentsToUse = allowedEquipments.Length > 0
                    ? allowedEquipments
                    : canParse
                        ? allEquipments.Where(eq => !creature.SkipEquipment(eq))
                        : allEquipments;

                foreach (var eq in equipmentsToUse)
                {
                    foreach (var stat in stats)
                    {
                        var next = new List<CompositionMonstersEntity>(current)
                {
                    new()
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
