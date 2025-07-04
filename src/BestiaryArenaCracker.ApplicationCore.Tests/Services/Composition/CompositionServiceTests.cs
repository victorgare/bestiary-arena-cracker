using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.ApplicationCore.Services.Composition;
using BestiaryArenaCracker.Domain;
using BestiaryArenaCracker.Domain.Entities;
using BestiaryArenaCracker.Domain.Room;
using NSubstitute;
using StackExchange.Redis;
using RedLockNet;
using System.Security.Cryptography;

namespace BestiaryArenaCracker.ApplicationCore.Tests.Services.Composition
{
    public class CompositionServiceTests
    {
        private IRoomConfigProvider _roomConfigProvider = null!;
        private ICompositionRepository _compositionRepository = null!;
        private IConnectionMultiplexer _connectionMultiplexer = null!;
        private IDistributedLockFactory _lockFactory = null!;

        [SetUp]
        public void Setup()
        {
            _roomConfigProvider = Substitute.For<IRoomConfigProvider>();
            _roomConfigProvider.BoostedRoomId.Returns(new HashSet<string>());
            _compositionRepository = Substitute.For<ICompositionRepository>();
            _connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
            _lockFactory = Substitute.For<IDistributedLockFactory>();
            var redLock = Substitute.For<IRedLock>();
            redLock.IsAcquired.Returns(true);
            _lockFactory.CreateLockAsync(Arg.Any<string>(), Arg.Any<TimeSpan>())
                      .Returns(Task.FromResult(redLock));

            var inUseField = typeof(CompositionService).GetField("_inUse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var dict = (System.Collections.Concurrent.ConcurrentDictionary<int, DateTime>)inUseField!.GetValue(null)!;
            dict.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            _connectionMultiplexer?.Dispose();
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
            _compositionRepository
                .GetNextAvailableCompositionsAsync(room.Id, Arg.Any<int>(), Arg.Any<IReadOnlySet<int>>())
                .Returns(Task.FromResult(new[] { composition }));
            _compositionRepository.GetMonstersByCompositionIdAsync(composition.Id).Returns(Task.FromResult(monsters));
            _compositionRepository.GetResultsCountAsync(composition.Id).Returns(Task.FromResult(0));
            var db = Substitute.For<IDatabase>();
            db.StringSetAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>(), Arg.Any<TimeSpan?>(), When.NotExists)
                .Returns(Task.FromResult(true));
            _connectionMultiplexer.GetDatabase().Returns(db);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository, _connectionMultiplexer, _lockFactory);

            var result = await service.FindCompositionAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Count.EqualTo(1));
                var first = result[0];
                Assert.That(first.Composition.Map, Is.EqualTo("Amber's Raft"));
                var board = first.Composition.Board.First();
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
            _compositionRepository
                .GetNextAvailableCompositionsAsync(room.Id, Arg.Any<int>(), Arg.Any<IReadOnlySet<int>>())
                .Returns(Task.FromResult(Array.Empty<CompositionEntity>()));
            _connectionMultiplexer.GetDatabase().Returns(Substitute.For<IDatabase>());

            var service = new CompositionService(_roomConfigProvider, _compositionRepository, _connectionMultiplexer, _lockFactory);

            var result = await service.FindCompositionAsync();

            Assert.That(result, Is.Empty);
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
            _compositionRepository.CompositionExistsAsync(room.Id, Arg.Any<string>()).Returns(Task.FromResult(false));
            _compositionRepository.AddCompositionAsync(Arg.Any<CompositionEntity>()).Returns(call =>
            {
                var entity = call.Arg<CompositionEntity>();
                entity.Id = 123;
                return Task.FromResult(entity);
            });
            _compositionRepository.AddMonstersAsync(Arg.Any<int>(), Arg.Any<IEnumerable<CompositionMonstersEntity>>())
                .Returns(Task.CompletedTask);

            var service = new CompositionService(_roomConfigProvider, _compositionRepository, _connectionMultiplexer, _lockFactory);

            // This will generate all possible compositions for the room
            await service.GenerateAllCompositionsForRoomAsync(room);

            // You can add assertions here based on how many times AddCompositionAsync/AddMonstersAsync were called, etc.
            await _compositionRepository.Received().AddCompositionAsync(Arg.Any<CompositionEntity>());
            await _compositionRepository.Received().AddMonstersAsync(Arg.Any<int>(), Arg.Any<IEnumerable<CompositionMonstersEntity>>());
        }

        [Test]
        public async Task ShouldNotGenerateOneCreatureCompositionsIfTheCreatureIsUselessSoloAndTheRoomAllowMultipleCreatures()
        {
            var data = new Data
            {
                Hitboxes = [false],
                Actors = []
            };

            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 2, File = new Domain.Room.File { Name = "Amber's Raft", Data = data } };
            _roomConfigProvider.Rooms.Returns([room]);
            _compositionRepository.CompositionExistsAsync(room.Id, Arg.Any<string>()).Returns(Task.FromResult(false));

            var cts = new CancellationTokenSource();

            _compositionRepository.AddCompositionAsync(Arg.Any<CompositionEntity>()).Returns(call =>
            {
                var entity = call.Arg<CompositionEntity>();
                entity.Id = 123;
                return Task.FromResult(entity);
            });

            _compositionRepository.AddMonstersAsync(Arg.Any<int>(), Arg.Any<IEnumerable<CompositionMonstersEntity>>())
                .Returns(call =>
                {
                    cts.Cancel(); // Cancel after adding monsters
                    return Task.CompletedTask;
                });

            var service = new CompositionService(_roomConfigProvider, _compositionRepository, _connectionMultiplexer, _lockFactory);

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

        [Test]
        public async Task GenerateAllCompositionsForRoomAsync_StoresExpectedCompositionData()
        {
            var data = new Data
            {
                Hitboxes = [false, false],
                Actors = []
            };

            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 1, File = new Domain.Room.File { Name = "Amber's Raft", Data = data } };
            _roomConfigProvider.Rooms.Returns([room]);

            CompositionEntity? storedEntity = null;
            IEnumerable<CompositionMonstersEntity>? storedMonsters = null;

            _compositionRepository.CompositionExistsAsync(room.Id, Arg.Any<string>()).Returns(Task.FromResult(false));
            _compositionRepository.AddCompositionAsync(Arg.Any<CompositionEntity>()).Returns(call =>
            {
                storedEntity = call.Arg<CompositionEntity>();
                storedEntity.Id = 1;
                return Task.FromResult(storedEntity);
            });

            var cts = new CancellationTokenSource();

            _compositionRepository.AddMonstersAsync(Arg.Any<int>(), Arg.Any<IEnumerable<CompositionMonstersEntity>>())
                .Returns(call =>
                {
                    storedMonsters = call.Arg<IEnumerable<CompositionMonstersEntity>>();
                    cts.Cancel();
                    return Task.CompletedTask;
                });

            var service = new CompositionService(_roomConfigProvider, _compositionRepository, _connectionMultiplexer, _lockFactory);

            await service.GenerateAllCompositionsForRoomAsync(room, cts.Token);

            var monster = storedMonsters!.Single();
            var raw = $"{room.Id}:" +
                      $"{monster.Name}-0-{monster.Hitpoints}-{monster.Attack}-{monster.AbilityPower}-{monster.Armor}-{monster.MagicResistance}-{monster.Level}-{monster.Equipment}-{monster.EquipmentStat}-{monster.EquipmentTier}";
            var expectedHash = Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(raw)));

            await Assert.MultipleAsync(async () =>
            {
                await _compositionRepository.Received(1).AddCompositionAsync(Arg.Any<CompositionEntity>());
                Assert.That(storedEntity!.RoomId, Is.EqualTo(room.Id));
                Assert.That(storedEntity.CompositionHash, Is.EqualTo(expectedHash));

                await _compositionRepository.Received(1).AddMonstersAsync(1, Arg.Any<IEnumerable<CompositionMonstersEntity>>());
                Assert.That(monster.Name, Is.EqualTo(Creatures.Bear.ToString()));
                Assert.That(monster.TileLocation, Is.EqualTo(0));
                Assert.That(monster.Equipment, Is.EqualTo(Equipments.DwarvenLegs));
                Assert.That(monster.EquipmentStat, Is.EqualTo(EquipmentStat.Hitpoints));
            });
        }

        [Test]
        public async Task GenerateAllCompositionsForRoomAsync_DoesNotCreateWhenHashExists()
        {
            var data = new Data
            {
                Hitboxes = [false],
                Actors = []
            };

            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 1, File = new Domain.Room.File { Name = "Amber's Raft", Data = data } };
            _roomConfigProvider.Rooms.Returns([room]);

            _compositionRepository.CompositionExistsAsync(room.Id, Arg.Any<string>()).Returns(Task.FromResult(true));

            var service = new CompositionService(_roomConfigProvider, _compositionRepository, _connectionMultiplexer, _lockFactory);

            await service.GenerateAllCompositionsForRoomAsync(room);

            await _compositionRepository.DidNotReceive().AddCompositionAsync(Arg.Any<CompositionEntity>());
            await _compositionRepository.DidNotReceive().AddMonstersAsync(Arg.Any<int>(), Arg.Any<IEnumerable<CompositionMonstersEntity>>());
        }

        [Test]
        public async Task FindCompositionAsync_ConcurrentRequests_DoNotReturnSameId()
        {
            var room = new RoomConfig { Id = "rkboat", MaxTeamSize = 1, File = new Domain.Room.File { Name = "Amber's Raft", Data = new Data() } };
            _roomConfigProvider.AllRooms.Returns([room]);

            var c1 = new CompositionEntity { Id = 1, RoomId = room.Id };
            var c2 = new CompositionEntity { Id = 2, RoomId = room.Id };

            var queue = new Queue<CompositionEntity?>(new[] { c1, c2 });
            _compositionRepository
                .GetNextAvailableCompositionsAsync(room.Id, Arg.Any<int>(), Arg.Any<IReadOnlySet<int>>())
                .Returns(call =>
                {
                    var excluded = call.ArgAt<IReadOnlySet<int>>(2);
                    while (queue.Count > 0 && queue.Peek() is { } next && excluded.Contains(next.Id))
                    {
                        queue.Dequeue();
                    }

                    return Task.FromResult(queue.Count > 0 ? new[] { queue.Dequeue()! } : Array.Empty<CompositionEntity>());
                });

            _compositionRepository.GetMonstersByCompositionIdAsync(Arg.Any<int>()).Returns(Task.FromResult(Array.Empty<CompositionMonstersEntity>()));
            _compositionRepository.GetResultsCountAsync(Arg.Any<int>()).Returns(Task.FromResult(0));

            var db = Substitute.For<IDatabase>();
            var reservationCount = new Dictionary<int, int>();
            db.StringSetAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>(), Arg.Any<TimeSpan?>(), When.NotExists)
                .Returns(call =>
                {
                    var key = (string)call.ArgAt<RedisKey>(0);
                    var id = int.Parse(key.Split(':')[1]);
                    if (!reservationCount.ContainsKey(id))
                    {
                        reservationCount[id] = 1;
                        return Task.FromResult(true);
                    }
                    reservationCount[id]++;
                    return Task.FromResult(false);
                });
            _connectionMultiplexer.GetDatabase().Returns(db);

            var service1 = new CompositionService(_roomConfigProvider, _compositionRepository, _connectionMultiplexer, _lockFactory);
            var service2 = new CompositionService(_roomConfigProvider, _compositionRepository, _connectionMultiplexer, _lockFactory);

            var results = await Task.WhenAll(service1.FindCompositionAsync(), service2.FindCompositionAsync());

            Assert.That(results[0], Has.Count.EqualTo(1), "first result should contain one item");
            Assert.That(results[1], Has.Count.EqualTo(1), "second result should contain one item");
            Assert.That(results[0][0].CompositionId, Is.Not.EqualTo(results[1][0].CompositionId));
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