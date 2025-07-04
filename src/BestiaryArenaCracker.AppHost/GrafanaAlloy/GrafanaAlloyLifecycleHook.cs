using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;

namespace BestiaryArenaCracker.AppHost.GrafanaAlloy
{
    internal sealed class GrafanaAlloyLifecycleHook : IDistributedApplicationLifecycleHook
    {
        private const string OtelExporterOtlpEndpoint = "OTEL_EXPORTER_OTLP_ENDPOINT";

        private readonly ILogger<GrafanaAlloyLifecycleHook> _logger;

        public GrafanaAlloyLifecycleHook(ILogger<GrafanaAlloyLifecycleHook> logger)
        {
            _logger = logger;
        }

        public Task AfterEndpointsAllocatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken)
        {
            var alloyResource = appModel.Resources.OfType<GrafanaAlloyResource>().FirstOrDefault();
            if (alloyResource == null)
            {
                _logger.LogWarning($"No {nameof(GrafanaAlloyResource)} resource found.");
                return Task.CompletedTask;
            }

            var endpoint = alloyResource.GetEndpoint(GrafanaAlloyResource.OtlpGrpcEndpointName);
            if (!endpoint.Exists)
            {
                _logger.LogWarning($"No {GrafanaAlloyResource.OtlpGrpcEndpointName} endpoint for the collector.");
                return Task.CompletedTask;
            }

            foreach (var resource in appModel.Resources)
            {
                resource.Annotations.Add(new EnvironmentCallbackAnnotation((EnvironmentCallbackContext context) =>
                {
                    if (context.EnvironmentVariables.ContainsKey(OtelExporterOtlpEndpoint))
                    {
                        _logger.LogDebug("Forwarding telemetry for {ResourceName} to the collector.", resource.Name);

                        context.EnvironmentVariables[OtelExporterOtlpEndpoint] = endpoint;
                    }
                }));
            }

            return Task.CompletedTask;
        }
    }
}
