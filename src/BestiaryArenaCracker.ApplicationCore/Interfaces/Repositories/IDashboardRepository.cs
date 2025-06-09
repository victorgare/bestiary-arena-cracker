using BestiaryArenaCracker.Domain.Dashboards;

namespace BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<SummaryDashboard[]> GetSummaryAsync();
    }
}
