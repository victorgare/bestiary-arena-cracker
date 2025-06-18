using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.ApplicationCore.Services.Dashboards;
using BestiaryArenaCracker.Domain.Dashboards;
using BestiaryArenaCracker.Domain.Room;
using NSubstitute;
using File = BestiaryArenaCracker.Domain.Room.File;

namespace BestiaryArenaCracker.ApplicationCore.Tests.Services.Dashboards;

public class DashboardServiceTests
{
    private IRoomConfigProvider _roomConfigProvider = null!;
    private IDashboardRepository _dashboardRepository = null!;

    [SetUp]
    public void Setup()
    {
        _roomConfigProvider = Substitute.For<IRoomConfigProvider>();
        _dashboardRepository = Substitute.For<IDashboardRepository>();
    }

    [Test]
    public async Task GetSummaryAsync_ReturnsTotalsUnchanged()
    {
        var roomConfig = new RoomConfig
        {
            Id = "rkboat",
            File = new File { Name = "Amber's Raft", Data = new Data() }
        };
        _roomConfigProvider.AllRooms.Returns([roomConfig]);

        var summary = new SummaryDashboard
        {
            RoomId = roomConfig.Id,
            RoomName = string.Empty,
            TotalResults = 15,
            TotalCompositions = 2,
            Ticks = 10,
            Points = 30,
            Grade = "S"
        };
        _dashboardRepository.GetSummaryAsync().Returns([summary]);

        var service = new DashboardService(_roomConfigProvider, _dashboardRepository);

        var result = await service.GetSummaryAsync();

        Assert.That(result, Has.Length.EqualTo(1));
        var res = result[0];
        Assert.Multiple(() =>
        {
            Assert.That(res.RoomId, Is.EqualTo(roomConfig.Id));
            Assert.That(res.RoomName, Is.EqualTo(roomConfig.Name));
            Assert.That(res.TotalResults, Is.EqualTo(summary.TotalResults));
            Assert.That(res.TotalCompositions, Is.EqualTo(summary.TotalCompositions));
            Assert.That(res.Ticks, Is.EqualTo(summary.Ticks));
            Assert.That(res.Points, Is.EqualTo(summary.Points));
        Assert.That(res.Grade, Is.EqualTo(summary.Grade));
        });
    }

    [Test]
    public async Task GetTimespanAsync_ReturnsRepositoryData()
    {
        var expected = new TimespanDashboard
        {
            TotalCompositions = 3,
            TotalResults = 5,
            Points = [new TimespanPoint { Date = DateTime.UtcNow.Date, Compositions = 1, Results = 2 }]
        };

        _dashboardRepository
            .GetTimespanAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(expected);

        var service = new DashboardService(_roomConfigProvider, _dashboardRepository);

        var result = await service.GetTimespanAsync(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        Assert.Multiple(() =>
        {
            Assert.That(result.TotalCompositions, Is.EqualTo(expected.TotalCompositions));
            Assert.That(result.TotalResults, Is.EqualTo(expected.TotalResults));
            Assert.That(result.Points, Has.Count.EqualTo(1));
            Assert.That(result.Points[0].Compositions, Is.EqualTo(expected.Points[0].Compositions));
            Assert.That(result.Points[0].Results, Is.EqualTo(expected.Points[0].Results));
        });
    }
}

