using BestiaryArenaCracker.ApplicationCore.Interfaces.Services;
using BestiaryArenaCracker.ApplicationCore.Services.Composition;

namespace BestiaryArenaCracker.Api.Infrastructure.DependencyInjection
{
    public static class AddServicesDependencyExtensions
    {
        public static void AddServicesDependency(this IServiceCollection services)
        {
            services.AddTransient<ICompositionService, CompositionService>();
        }
    }
}
