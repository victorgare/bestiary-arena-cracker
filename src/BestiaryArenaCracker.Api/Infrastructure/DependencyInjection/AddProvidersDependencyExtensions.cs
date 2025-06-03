using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Interfaces.Repositories;
using BestiaryArenaCracker.ApplicationCore.Providers;
using BestiaryArenaCracker.Repository.Context;

namespace BestiaryArenaCracker.Api.Infrastructure.DependencyInjection
{
    public static class AddProvidersDependencyExtensions
    {
        public static void AddProvidersDependency(this IServiceCollection services)
        {
            services.AddSingleton<IRoomConfigProvider, RoomConfigProvider>();
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        }
    }
}
