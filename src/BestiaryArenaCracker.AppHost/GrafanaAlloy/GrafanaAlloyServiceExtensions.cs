using Aspire.Hosting.Lifecycle;

namespace BestiaryArenaCracker.AppHost.GrafanaAlloy
{
    public static class GrafanaAlloyServiceExtensions
    {
        public static IDistributedApplicationBuilder AddGrafanaAlloyInfrastructure(this IDistributedApplicationBuilder builder)
        {
            builder.Services.TryAddLifecycleHook<GrafanaAlloyLifecycleHook>();

            return builder;
        }
    }
}
