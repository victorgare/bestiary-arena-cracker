using BestiaryArenaCracker.Domain.Dashboards;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<SummaryDashboard[]> GetSummaryAsync();
        Task<RoomDetailsDashboard> GetRoomDetailsAsync(string roomId);
    }
}
