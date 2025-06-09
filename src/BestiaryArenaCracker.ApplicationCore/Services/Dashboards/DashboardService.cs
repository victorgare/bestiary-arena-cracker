using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;
using BestiaryArenaCracker.Domain.Dashboards;

namespace BestiaryArenaCracker.ApplicationCore.Services.Dashboards
{
    public class DashboardService(
        IRoomConfigProvider roomConfigProvider,
        IDashboardRepository dashboardRepository,
        ICompositionService compositionService) : IDashboardService
    {
        public async Task<SummaryDashboard[]> GetSummaryAsync()
        {
            var roomsSummary = await dashboardRepository.GetSummaryAsync();

            foreach (var roomSummary in roomsSummary)
            {
                var room = roomConfigProvider.Rooms.FirstOrDefault(x => x.Id == roomSummary.RoomId);

                roomSummary.RoomName = room!.Name;
                roomSummary.TotalCompositions = await compositionService.CalculatePossibleCompositions(room);
            }

            return roomsSummary;
        }
    }
}
