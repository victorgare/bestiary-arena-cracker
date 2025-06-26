using Aspire.Hosting.Lifecycle;

namespace BestiaryArenaCracker.AppHost.OpenTelemetryCollector
{
    public static class OpenTelemetryCollectorServiceExtensions
    {
        public static IDistributedApplicationBuilder AddOpenTelemetryCollectorInfrastructure(this IDistributedApplicationBuilder builder)
        {
            builder.Services.TryAddLifecycleHook<OpenTelemetryCollectorLifecycleHook>();

            return builder;
        }
    }
}
