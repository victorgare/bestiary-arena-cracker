using BestiaryArenaCracker.Domain.Entities;

namespace BestiaryArenaCracker.Domain.Dashboards
{
    public class CompositionDashboardResult
    {
        public int CompositionId { get; set; }
        public int? MinTicks { get; set; }
        public int? MaxPoints { get; set; }
        public int? TotalResults { get; set; }
        public int? VictoryCount { get; set; }
        public double? VictoryRate { get; set; }
        public List<CompositionMonstersEntity> Monsters { get; set; } = [];
    }
}
