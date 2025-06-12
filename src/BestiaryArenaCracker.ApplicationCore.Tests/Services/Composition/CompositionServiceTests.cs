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
        public async Task FindCompositionAsync_ReturnsExistingComposition()
        {
            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 1, File = new Domain.Room.File { Name = "Amber's Raft", Data = new Data() } };
            var composition = new CompositionEntity { Id = 1, RoomId = "rkboat", CompositionHash = "hash1" };
            var monsters = new[]
            {
                new CompositionMonstersEntity
                {
                    CompositionId = 1,
                    Name = Creatures.MinotaurMage.ToString(),
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

            _roomConfigProvider.AllRooms.Returns([room]);
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
        public async Task FindCompositionAsync_ReturnsNull_WhenNoCompositionsAvailable()
        {
            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 1, File = new Domain.Room.File { Name = "Amber's Raft", Data = new Data() } };
            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.GetNextAvailableCompositionAsync(room.Id, Arg.Any<int>()).Returns((CompositionEntity?)null);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);

            var result = await service.FindCompositionAsync();

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GenerateAllCompositionsForRoomAsync_GeneratesAndStoresCompositions()
        {
            var data = new Data
            {
                Hitboxes = [false],
                Actors = []
            };

            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 1, File = new Domain.Room.File { Name = "Amber's Raft", Data = data } };
            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.CompositionExistsAsync(room.Id, Arg.Any<string>()).Returns(false);
            _compositionRepository.AddCompositionAsync(Arg.Any<CompositionEntity>()).Returns(call =>
            {
                var entity = call.Arg<CompositionEntity>();
                entity.Id = 123;
                return entity;
            });
            _compositionRepository.AddMonstersAsync(Arg.Any<int>(), Arg.Any<IEnumerable<CompositionMonstersEntity>>())
                .Returns(Task.CompletedTask);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);

            // This will generate all possible compositions for the room
            await service.GenerateAllCompositionsForRoomAsync(room);

            // You can add assertions here based on how many times AddCompositionAsync/AddMonstersAsync were called, etc.
            await _compositionRepository.Received().AddCompositionAsync(Arg.Any<CompositionEntity>());
            await _compositionRepository.Received().AddMonstersAsync(Arg.Any<int>(), Arg.Any<IEnumerable<CompositionMonstersEntity>>());
        }

        [Test]
        public async Task ShouldNotGenerateOneCreatureCompositionsIfTheCreatureIsUseslessSoloAndTheRoomAllowMultipleCreatures()
        {
            var data = new Data
            {
                Hitboxes = [false],
                Actors = []
            };

            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 2, File = new Domain.Room.File { Name = "Amber's Raft", Data = data } };
            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.CompositionExistsAsync(room.Id, Arg.Any<string>()).Returns(false);

            var cts = new CancellationTokenSource();

            _compositionRepository.AddCompositionAsync(Arg.Any<CompositionEntity>()).Returns(call =>
            {
                var entity = call.Arg<CompositionEntity>();
                entity.Id = 123;
                return entity;
            });

            _compositionRepository.AddMonstersAsync(Arg.Any<int>(), Arg.Any<IEnumerable<CompositionMonstersEntity>>())
                .Returns(call =>
                {
                    cts.Cancel(); // Cancel after adding monsters
                    return Task.CompletedTask;
                });

            var service = new CompositionService(_roomConfigProvider, _compositionRepository);

            await service.GenerateAllCompositionsForRoomAsync(room, cts.Token);

            // assert
            await Assert.MultipleAsync(async () =>
            {
                await _compositionRepository.Received(1).AddCompositionAsync(Arg.Any<CompositionEntity>());
                await _compositionRepository.Received(1).AddMonstersAsync(
                    Arg.Any<int>(),
                    Arg.Is<IEnumerable<CompositionMonstersEntity>>(monsters => IsNotSoloUseless(monsters))
                );
            });
        }

        // Fix the predicate to return true for non-solo-useless creatures
        private static bool IsNotSoloUseless(IEnumerable<CompositionMonstersEntity> monsters)
        {
            var list = monsters.ToList();
            if (list.Count != 1)
                return false;

            var soloUselessNames = typeof(Creatures)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.GetCustomAttributes(typeof(Domain.Attributes.SoloUselessAttribute), false).Length != 0)
                .Select(f => f.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Return true if NOT solo useless
            return !soloUselessNames.Contains(list[0].Name);
        }
    }
}