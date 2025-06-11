using BestiaryArenaCracker.Api.Workers;

namespace BestiaryArenaCracker.Api.Infrastructure.DependencyInjection
{
    public static class AddWorkersDependencyExtensions
    {
        public static void AddWorkersDependency(this IServiceCollection services)
        {
            services.AddHostedService<GenerateCompositionsWorker>();
        }
    }
}
