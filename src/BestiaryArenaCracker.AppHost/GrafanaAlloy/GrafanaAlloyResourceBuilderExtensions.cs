using BestiaryArenaCracker.AppHost.Extensions;
using Microsoft.Extensions.Hosting;

namespace BestiaryArenaCracker.AppHost.GrafanaAlloy
{
    public static class GrafanaAlloyResourceBuilderExtensions
    {
        private const string DashboardOtlpUrlVariableName = "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL";
        private const string DashboardOtlpApiKeyVariableName = "AppHost:OtlpApiKey";
        private const string DashboardOtlpUrlDefaultValue = "http://localhost:18889";
        private const string AlloyImageName = "grafana/alloy";
        private const string AlloyImageTag = "latest";
        private const int PrometheusPort = 8889;

        public static IResourceBuilder<GrafanaAlloyResource> AddGrafanaAlloy(this IDistributedApplicationBuilder builder, string name, string configFileLocation)
        {
            builder.AddGrafanaAlloyInfrastructure();

            var url = builder.Configuration[DashboardOtlpUrlVariableName] ?? DashboardOtlpUrlDefaultValue;
            var isHttpsEnabled = url.StartsWith("https", StringComparison.OrdinalIgnoreCase);

            var dashboardOtlpEndpoint = new HostUrl(url);

            var resource = new GrafanaAlloyResource(name);
            var resourceBuilder = builder.AddResource(resource)
                .WithImage(AlloyImageName, AlloyImageTag)
                .WithEndpoint(targetPort: 4317, name: GrafanaAlloyResource.OtlpGrpcEndpointName, scheme: isHttpsEnabled ? "https" : "http")
                .WithEndpoint(targetPort: 4318, name: GrafanaAlloyResource.OtlpHttpEndpointName, scheme: isHttpsEnabled ? "https" : "http")
                .WithEndpoint(targetPort: PrometheusPort, name: GrafanaAlloyResource.PrometheusEndpointName)
                .WithBindMount(configFileLocation, "/etc/alloy/config.alloy")
                .WithArgs("run", "--config.file=/etc/alloy/config.alloy")
                .WithEnvironment("ASPIRE_ENDPOINT", $"{dashboardOtlpEndpoint}")
                .WithEnvironment("ASPIRE_API_KEY", builder.Configuration[DashboardOtlpApiKeyVariableName])
                .WithEnvironment("ASPIRE_INSECURE", isHttpsEnabled ? "false" : "true");

            if (isHttpsEnabled && builder.ExecutionContext.IsRunMode && builder.Environment.IsDevelopment())
            {
                DevCertHostingExtensions.RunWithHttpsDevCertificate(resourceBuilder, "HTTPS_CERT_FILE", "HTTPS_CERT_KEY_FILE");
            }

            return resourceBuilder;
        }
    }
}
