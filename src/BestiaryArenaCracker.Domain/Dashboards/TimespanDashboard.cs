using System;
using System.Collections.Generic;

namespace BestiaryArenaCracker.Domain.Dashboards
{
    public class TimespanPoint
    {
        public DateTime Date { get; set; }
        public int Compositions { get; set; }
        public int Results { get; set; }
    }

    public class TimespanDashboard
    {
        public int TotalCompositions { get; set; }
        public int TotalResults { get; set; }
        public List<TimespanPoint> Points { get; set; } = [];
    }
}
