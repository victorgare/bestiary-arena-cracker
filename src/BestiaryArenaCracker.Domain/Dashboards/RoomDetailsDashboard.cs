namespace BestiaryArenaCracker.Domain.Dashboards
{
    public class RoomDetailsDashboard
    {
        public List<CompositionDashboardResult> LeastTicks { get; set; } = [];
        public List<CompositionDashboardResult> HighestPoints { get; set; } = [];
        public List<CompositionDashboardResult> HighestVictoryRate { get; set; } = [];
    }
}
