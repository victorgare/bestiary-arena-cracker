using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.Domain.Dashboards;
using Microsoft.EntityFrameworkCore;

namespace BestiaryArenaCracker.Repository.Repositories
{
    public class DashboardRepository(IApplicationDbContext dbContext) : IDashboardRepository
    {
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
