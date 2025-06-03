using BestiaryArenaCracker.ApplicationCore.Interfaces.Providers;
using BestiaryArenaCracker.ApplicationCore.Providers;

namespace BestiaryArenaCracker.Api.Infrastructure.DependencyInjection
{
    public static class AddProvidersDependencyExtensions
    {
        public static void AddProvidersDependency(this IServiceCollection services)
        {
            services.AddSingleton<IRoomConfigProvider, RoomConfigProvider>();
        }
    }
}
