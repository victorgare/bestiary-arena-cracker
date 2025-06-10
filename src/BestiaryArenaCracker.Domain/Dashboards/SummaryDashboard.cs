namespace BestiaryArenaCracker.Domain.Dashboards
{
    public class SummaryDashboard
    {
        public string RoomId { get; set; } = null!;
        public string RoomName { get; set; } = null!;
        public int TotalResults { get; set; }
        public Int128 TotalCompositions { get; set; }
        public int Ticks { get; set; }
        public int Points { get; set; }
        public string Grade { get; set; } = null!;
    }
}
