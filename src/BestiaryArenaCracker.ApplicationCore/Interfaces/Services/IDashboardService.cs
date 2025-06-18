using System;
using BestiaryArenaCracker.Domain.Dashboards;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<SummaryDashboard[]> GetSummaryAsync();
        Task<RoomDetailsDashboard> GetRoomDetailsAsync(string roomId);
        Task<TimespanDashboard> GetTimespanAsync(DateTime start, DateTime end);
    }
}
