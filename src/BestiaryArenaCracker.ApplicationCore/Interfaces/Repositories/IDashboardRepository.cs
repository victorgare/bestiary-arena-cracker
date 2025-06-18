using System;
using BestiaryArenaCracker.Domain.Dashboards;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<SummaryDashboard[]> GetSummaryAsync();
        Task<RoomDetailsDashboard> GetRoomDetailsAsync(string roomId);
        Task<TimespanDashboard> GetTimespanAsync(DateTime start, DateTime end);
    }
}
