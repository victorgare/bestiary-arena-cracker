using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.Repository.Repositories;

namespace BestiaryArenaCracker.Api.Infrastructure.DependencyInjection
{
    public static class AddRepositoriesDependencyExtensions
    {
        public static void AddRepositoriesDependency(this IServiceCollection services)
        {
            services.AddTransient<ICompositionRepository, CompositionRepository>();
            services.AddTransient<IDashboardRepository, DashboardRepository>();
        }
    }
}
