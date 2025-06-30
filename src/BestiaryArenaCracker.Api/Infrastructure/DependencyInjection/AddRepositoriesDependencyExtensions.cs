using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.Repository.Repositories;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace BestiaryArenaCracker.Api.Infrastructure.DependencyInjection
{
    public static class AddRepositoriesDependencyExtensions
    {
        public static void AddRepositoriesDependency(this IServiceCollection services)
        {
            services.AddTransient<ICompositionRepository, CompositionRepository>();
            services.AddTransient<IDashboardRepository, DashboardRepository>();

            services.AddSingleton<IDistributedLockFactory>(sp =>
            {
                var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                return RedLockFactory.Create(new List<RedLockMultiplexer> {
                    new(redis )
                });
            });
        }
    }
}
