namespace BestiaryArenaCracker.AppHost.GrafanaAlloy
{
    public class GrafanaAlloyResource(string name) : ContainerResource(name)
    {
        internal const string OtlpGrpcEndpointName = "grpc";
        internal const string OtlpHttpEndpointName = "http";
        internal const string PrometheusEndpointName = "prometheus";
    }
}
