using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.Domain.Dashboards;
using Microsoft.EntityFrameworkCore;

namespace BestiaryArenaCracker.Repository.Repositories
{
    public class DashboardRepository(IApplicationDbContext dbContext) : IDashboardRepository
    {
        public async Task<RoomDetailsDashboard> GetRoomDetailsAsync(string roomId)
        {
            // Query base: compositions for the room
            var compositionsQuery = dbContext.Compositions
                .Where(c => c.RoomId == roomId);

            // Compose stats for each composition
            var statsQuery = dbContext.CompositionResults
                .Where(r => compositionsQuery.Select(c => c.Id).Contains(r.CompositionId))
                .GroupBy(r => r.CompositionId)
                .Select(g => new
                {
                    CompositionId = g.Key,
                    MinTicks = g.Where(x => x.Victory).Select(x => (int?)x.Ticks).Min(),
                    MaxPoints = g.Where(x => x.Victory).Select(x => (int?)x.Points).Max(),
                    TotalResults = g.Count(),
                    VictoryCount = g.Count(x => x.Victory),
                    VictoryRate = g.Count(x => x.Victory) * 1.0 / g.Count()
                });

            // Join with composition hash
            var statsWithHashQuery = from s in statsQuery
                                     join c in dbContext.Compositions on s.CompositionId equals c.Id
                                     select new
                                     {
                                         s.CompositionId,
                                         s.MinTicks,
                                         s.MaxPoints,
                                         s.TotalResults,
                                         s.VictoryCount,
                                         s.VictoryRate
                                     };

            // Get top 10 for each category (project to DTO, but without monsters yet)
            var leastTicks = await statsWithHashQuery
                .Where(x => x.MinTicks != null)
                .OrderBy(x => x.MinTicks)
                .Take(10)
                .Select(x => new CompositionDashboardResult
                {
                    CompositionId = x.CompositionId,
                    MinTicks = x.MinTicks,
                    MaxPoints = x.MaxPoints,
                    TotalResults = x.TotalResults,
                    VictoryCount = x.VictoryCount,
                    VictoryRate = x.VictoryRate,
                })
                .ToListAsync();

            var highestPoints = await statsWithHashQuery
                .Where(x => x.MaxPoints != null)
                .OrderByDescending(x => x.MaxPoints)
                .ThenBy(x => x.MinTicks)
                .Take(10)
                .Select(x => new CompositionDashboardResult
                {
                    CompositionId = x.CompositionId,
                    MinTicks = x.MinTicks,
                    MaxPoints = x.MaxPoints,
                    TotalResults = x.TotalResults,
                    VictoryCount = x.VictoryCount,
                    VictoryRate = x.VictoryRate,
                })
                .ToListAsync();

            var highestVictoryRate = await statsWithHashQuery
                .OrderByDescending(x => x.VictoryRate)
                .ThenByDescending(x => x.VictoryCount)
                .Take(10)
                .Select(x => new CompositionDashboardResult
                {
                    CompositionId = x.CompositionId,
                    MinTicks = x.MinTicks,
                    MaxPoints = x.MaxPoints,
                    TotalResults = x.TotalResults,
                    VictoryCount = x.VictoryCount,
                    VictoryRate = x.VictoryRate,
                })
                .ToListAsync();

            // Gather all unique composition IDs from the three lists
            var allCompIds = leastTicks.Select(x => x.CompositionId)
                .Concat(highestPoints.Select(x => x.CompositionId))
                .Concat(highestVictoryRate.Select(x => x.CompositionId))
                .Distinct()
                .ToList();

            // Fetch all monsters for these compositions in one query
            var monsters = await dbContext.CompositionMonsters
                .Where(m => allCompIds.Contains(m.CompositionId))
                .ToListAsync();

            // Attach monsters to each result
            void AttachMonsters(List<CompositionDashboardResult> results)
            {
                foreach (var result in results)
                {
                    result.Monsters = monsters.Where(m => m.CompositionId == result.CompositionId).ToList();
                }
            }

            AttachMonsters(leastTicks);
            AttachMonsters(highestPoints);
            AttachMonsters(highestVictoryRate);

            return new RoomDetailsDashboard
            {
                LeastTicks = leastTicks,
                HighestPoints = highestPoints,
                HighestVictoryRate = highestVictoryRate
            };
        }

        public Task<SummaryDashboard[]> GetSummaryAsync()
        {
            // Subquery: compositions per room
            var compositionsPerRoom = dbContext.Compositions
                .GroupBy(c => c.RoomId)
                .Select(g => new
                {
                    RoomId = g.Key,
                    TotalCompositions = g.Count()
                });

            // Subquery: results per room (via compositions)
            var resultsPerRoom = from composition in dbContext.Compositions
                                 join result in dbContext.CompositionResults
                                    on composition.Id equals result.CompositionId
                                 group result by composition.RoomId into g
                                 select new
                                 {
                                     RoomId = g.Key,
                                     TotalResults = g.Count(),
                                     Ticks = g.Where(r => r.Victory).OrderBy(r => r.Ticks).Select(r => (int?)r.Ticks).FirstOrDefault() ?? 0,
                                     Points = g.Where(r => r.Victory).Max(r => (int?)r.Points) ?? 0,
                                     Grade = g.Where(r => r.Victory).OrderByDescending(r => r.Points).Select(r => r.Grade).FirstOrDefault() ?? "F"
                                 };

            // Join the two subqueries
            var query = from results in resultsPerRoom
                        join comps in compositionsPerRoom on results.RoomId equals comps.RoomId
                        select new SummaryDashboard
                        {
                            RoomId = results.RoomId,
                            TotalResults = comps.TotalCompositions,
                            Ticks = results.Ticks,
                            Points = results.Points,
                            Grade = results.Grade
                        };

            return query.ToArrayAsync();
        }
    }
}
