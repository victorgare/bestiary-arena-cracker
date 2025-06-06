using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.ApplicationCore.Services.Composition;
using BestiaryArenaCracker.Domain;
using BestiaryArenaCracker.Domain.Entities;
using BestiaryArenaCracker.Domain.Room;
using NSubstitute;

namespace BestiaryArenaCracker.ApplicationCore.Tests.Services.Composition
{
    public class CompositionServiceTests
    {
        private IRoomConfigProvider _roomConfigProvider = null!;
        private ICompositionRepository _compositionRepository = null!;

        [SetUp]
        public void Setup()
        {
            _roomConfigProvider = Substitute.For<IRoomConfigProvider>();
            _compositionRepository = Substitute.For<ICompositionRepository>();
        }

        [Test]
        public async Task GetNextOrGenerate_ReturnsExistingComposition_WhenUnderMaxResults()
        {
            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 1, File = new Domain.Room.File { Name = "Amber's Raft", Data = new Data() } };
            var composition = new CompositionEntity { Id = 1, RoomId = "rkboat", CompositionHash = "hash1" };

            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.GetNextAvailableCompositionAsync(room.Id, Arg.Any<int>()).Returns(composition);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);

            var result = await service.GetNextOrGenerate(room);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!.Id, Is.EqualTo(composition.Id));
            });
        }

        [Test]
        public async Task GetNextOrGenerate_GeneratesNewComposition_WhenNoneExist()
        {
            var data = new Data
            {
                Hitboxes = [false],
                Actors = []
            };

            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 1, File = new Domain.Room.File { Name = "Amber's Raft", Data = data } };
            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.GetNextAvailableCompositionAsync(room.Id, Arg.Any<int>()).Returns((CompositionEntity?)null);
            _compositionRepository.AddCompositionAsync(Arg.Any<CompositionEntity>()).Returns(call =>
            {
                var entity = call.Arg<CompositionEntity>();
                entity.Id = 123;
                return entity;
            });
            _compositionRepository.AddMonstersAsync(Arg.Any<int>(), Arg.Any<IEnumerable<CompositionMonstersEntity>>())
                .Returns(Task.CompletedTask);
            _compositionRepository.CompositionExistsAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);

            var result = await service.GetNextOrGenerate(room);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!.RoomId, Is.EqualTo(room.Id));
                Assert.That(result.Id, Is.EqualTo(123));
            });
        }

        [Test]
        public async Task FindCompositionAsync_ReturnsExpectedDtoShape()
        {
            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 1, File = new Domain.Room.File { Name = "Amber's Raft", Data = new Data() } };
            var composition = new CompositionEntity { Id = 1, RoomId = "rkboat", CompositionHash = "hash1" };
            var monsters = new[]
            {
                new CompositionMonstersEntity
                {
                    CompositionId = 1,
                    Name = "Minotaur Mage",
                    Hitpoints = 20,
                    Attack = 20,
                    AbilityPower = 20,
                    Armor = 20,
                    MagicResistance = 20,
                    Level = 50,
                    TileLocation = 67,
                    Equipment = Equipments.DwarvenHelmet,
                    EquipmentStat = EquipmentStat.Hitpoints,
                    EquipmentTier = 3
                }
            };

            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.GetNextAvailableCompositionAsync(room.Id, Arg.Any<int>()).Returns(composition);
            _compositionRepository.GetMonstersByCompositionIdAsync(composition.Id).Returns(monsters);
            _compositionRepository.GetResultsCountAsync(composition.Id).Returns(0);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);

            var result = await service.FindCompositionAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!.Composition.Map, Is.EqualTo("Amber's Raft"));
                var board = result.Composition.Board.First();
                Assert.That(board.Tile, Is.EqualTo(67));
                Assert.That(board.Monster.Name, Is.EqualTo("minotaur mage"));
                Assert.That(board.Monster.Hp, Is.EqualTo(20));
                Assert.That(board.Equipment.Name, Is.EqualTo("dwarven helmet"));
                Assert.That(board.Equipment.Stat, Is.EqualTo("hp"));
                Assert.That(board.Equipment.Tier, Is.EqualTo(3));
            });
        }

        [Test]
        public async Task GetNextOrGenerate_DoesNotReturnDuplicateCompositions_ByHash()
        {
            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 1, File = new Domain.Room.File { Name = "Amber's Raft", Data = new Data() } };
            var existingHash = "existing-hash";
            var composition = new CompositionEntity { Id = 1, RoomId = "rkboat", CompositionHash = existingHash };

            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.GetNextAvailableCompositionAsync(room.Id, Arg.Any<int>()).Returns(composition);
            _compositionRepository.CompositionExistsAsync(room.Id, existingHash).Returns(true);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);

            var result = await service.GetNextOrGenerate(room);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.CompositionHash, Is.EqualTo(existingHash));
        }

        [Test]
        public async Task GetNextOrGenerate_DoesNotAllowDuplicateCreaturesInTeam()
        {
            // Arrange
            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 2, File = new Domain.Room.File { Name = "Amber's Raft", Data = new Data() } };
            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.GetNextAvailableCompositionAsync(room.Id, Arg.Any<int>()).Returns((CompositionEntity?)null);
            _compositionRepository.AddCompositionAsync(Arg.Any<CompositionEntity>()).Returns(call => call.Arg<CompositionEntity>());
            var capturedMonsters = new List<CompositionMonstersEntity>();
            _compositionRepository.AddMonstersAsync(Arg.Any<int>(), Arg.Do<IEnumerable<CompositionMonstersEntity>>(m => capturedMonsters = m.ToList()))
                .Returns(Task.CompletedTask);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);

            await service.GetNextOrGenerate(room);

            // Assert: No duplicate creature names in the team
            var names = capturedMonsters.Select(m => m.Name).ToList();
            Assert.That(names.Distinct().Count(), Is.EqualTo(names.Count), "Team contains duplicate creatures");
        }

        [Test]
        public async Task GetNextOrGenerate_ExcludesSoloUselessCreatures_WhenTeamSizeIsOne()
        {
            // Arrange
            var soloUselessCreatureName = "Slime"; // This should match your enum and attribute
            var room = new RoomConfig
            {
                Id = "rkboat",
                MaxTeamSize = 1,
                File = new Domain.Room.File
                {
                    Name = "Amber's Raft",
                    Data = new Data
                    {
                        Hitboxes = [false],
                        Actors = [],
                    }
                }
            };

            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.GetNextAvailableCompositionAsync(room.Id, Arg.Any<int>()).Returns((CompositionEntity?)null);
            _compositionRepository.AddCompositionAsync(Arg.Any<CompositionEntity>()).Returns(call => call.Arg<CompositionEntity>());
            var capturedMonsters = new List<CompositionMonstersEntity>();
            _compositionRepository.AddMonstersAsync(Arg.Any<int>(), Arg.Do<IEnumerable<CompositionMonstersEntity>>(m => capturedMonsters = m.ToList()))
                .Returns(Task.CompletedTask);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);

            await service.GetNextOrGenerate(room);

            Assert.That(capturedMonsters, Is.Not.Empty, "No monsters were generated for the solo team.");
            Assert.That(
                capturedMonsters.All(m => !string.Equals(m.Name, soloUselessCreatureName, StringComparison.OrdinalIgnoreCase)),
                $"Solo-useless creature '{soloUselessCreatureName}' was included in a solo team."
            );
        }

        [Test]
        public async Task GetNextOrGenerate_PlacesMonstersOnlyOnFreeTiles()
        {
            // Arrange
            var data = new Data
            {
                Hitboxes = [false, false, false],
                Actors = [],
            };

            var room = new RoomConfig
            {
                Id = "rkboat",
                MaxTeamSize = 2,
                File = new Domain.Room.File { Name = "Amber's Raft", Data = data }
            };

            // Dynamically get the free tiles from the Data instance to match the service logic
            int[] freeTiles = data.GetFreeTiles();

            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.GetNextAvailableCompositionAsync(room.Id, Arg.Any<int>()).Returns((CompositionEntity?)null);
            _compositionRepository.AddCompositionAsync(Arg.Any<CompositionEntity>()).Returns(call => call.Arg<CompositionEntity>());
            var capturedMonsters = new List<CompositionMonstersEntity>();
            _compositionRepository.AddMonstersAsync(Arg.Any<int>(), Arg.Do<IEnumerable<CompositionMonstersEntity>>(m => capturedMonsters = m.ToList()))
                .Returns(Task.CompletedTask);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);

            await service.GetNextOrGenerate(room);

            Assert.That(capturedMonsters, Is.Not.Empty);
            Assert.That(capturedMonsters.All(m => freeTiles.Contains(m.TileLocation)), "A monster was placed on a non-free tile");
        }

        [Test]
        public async Task GetNextOrGenerate_ProducesAllEquipmentAndStatCombinations_ForSmallTeam()
        {
            // Arrange
            var room = new RoomConfig
            {
                Id = "rkboat",
                MaxTeamSize = 1,
                File = new Domain.Room.File
                {
                    Name = "Amber's Raft",
                    Data = new Data
                    {
                        Hitboxes = [false],
                        Actors = [],
                    }
                }
            };

            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.GetNextAvailableCompositionAsync(room.Id, Arg.Any<int>()).Returns((CompositionEntity?)null);

            int compositionId = 1;
            var generatedHashes = new HashSet<string>();
            _compositionRepository.AddCompositionAsync(Arg.Any<CompositionEntity>()).Returns(call =>
            {
                var entity = call.Arg<CompositionEntity>();
                entity.Id = compositionId++;
                // Side effect: record the hash
                generatedHashes.Add(entity.CompositionHash);
                return entity;
            });

            _compositionRepository.CompositionExistsAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns(call => generatedHashes.Contains(call.ArgAt<string>(1)));

            var capturedMonsters = new List<CompositionMonstersEntity>();
            _compositionRepository.AddMonstersAsync(Arg.Any<int>(), Arg.Do<IEnumerable<CompositionMonstersEntity>>(m => capturedMonsters = m.ToList()))
                .Returns(Task.CompletedTask);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);

            int equipmentCount = Enum.GetValues<Equipments>().Length;
            int statCount = Enum.GetValues<EquipmentStat>().Length;
            int expected = equipmentCount * statCount;
            var allCombos = new HashSet<(Equipments, EquipmentStat)>();

            for (int i = 0; i < expected * 2; i++)
            {
                capturedMonsters.Clear();
                await service.GetNextOrGenerate(room);
                foreach (var m in capturedMonsters)
                    allCombos.Add((m.Equipment, m.EquipmentStat));
                if (allCombos.Count == expected)
                    break;
            }

            Assert.That(allCombos.Count, Is.EqualTo(expected), $"Expected {expected} unique equipment/stat combos, got {allCombos.Count}");
        }

        [Test]
        public async Task GetNextOrGenerate_ReturnsNull_WhenNoValidTeams()
        {
            // No free tiles, so no valid teams can be generated
            var data = new Data { Hitboxes = [true, true], Actors = [] };
            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 1, File = new Domain.Room.File { Name = "Amber's Raft", Data = data } };
            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.GetNextAvailableCompositionAsync(room.Id, Arg.Any<int>()).Returns((CompositionEntity?)null);
            _compositionRepository.AddCompositionAsync(Arg.Any<CompositionEntity>()).Returns(call => call.Arg<CompositionEntity>());
            _compositionRepository.AddMonstersAsync(Arg.Any<int>(), Arg.Any<IEnumerable<CompositionMonstersEntity>>()).Returns(Task.CompletedTask);
            _compositionRepository.CompositionExistsAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);
            var result = await service.GetNextOrGenerate(room);

            Assert.That(result, Is.Null, "Expected null when no valid teams can be generated");
        }
    }
}