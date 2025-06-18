using System;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;
using BestiaryArenaCracker.Domain.Dashboards;

namespace BestiaryArenaCracker.ApplicationCore.Services.Dashboards
{
    public class DashboardService(
        IRoomConfigProvider roomConfigProvider,
        IDashboardRepository dashboardRepository) : IDashboardService
    {
        public Task<RoomDetailsDashboard> GetRoomDetailsAsync(string roomId)
        {
            return dashboardRepository.GetRoomDetailsAsync(roomId);
        }

        public async Task<SummaryDashboard[]> GetSummaryAsync()
        {
            var roomsSummary = await dashboardRepository.GetSummaryAsync();

            var result = new List<SummaryDashboard>();
            foreach (var roomConfig in roomConfigProvider.AllRooms)
            {
                var roomSummary = roomsSummary.FirstOrDefault(x => x.RoomId == roomConfig.Id);

                if (roomSummary != null)
                {
                    roomSummary.RoomName = roomConfig.Name;
                }
                else
                {
                    roomSummary = new SummaryDashboard
                    {
                        RoomId = roomConfig.Id,
                        RoomName = roomConfig.Name,
                        TotalResults = 0,
                        TotalCompositions = 0,
                        Ticks = 0,
                        Points = 0,
                        Grade = "F"
                    };
                }

                result.Add(roomSummary);
            }

            return [.. result];
        }

        public Task<TimespanDashboard> GetTimespanAsync(DateTime start, DateTime end)
        {
            return dashboardRepository.GetTimespanAsync(start, end);
        }
    }
}
