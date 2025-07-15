using BestiaryArenaCracker.AppHost.OpenTelemetryCollector;

var builder = DistributedApplication.CreateBuilder(args);

var prometheus = builder
    .AddContainer("prometheus", "prom/prometheus", "v3.2.1")
    .WithBindMount("../../infra/prometheus", "/etc/prometheus", isReadOnly: true)
    .WithArgs("--web.enable-otlp-receiver", "--config.file=/etc/prometheus/prometheus.yml")
    .WithHttpEndpoint(targetPort: 9090, name: "http");

var loki = builder.AddContainer("loki", "grafana/loki:latest")
    .WithBindMount("../../infra/loki", "/etc/loki")
    .WithArgs("-config.file=/etc/loki/loki-config.yml")
    .WithArgs("-config.expand-env=true")
    .WithHttpEndpoint(port: 5400, targetPort: 3100, name: "http");

var nodeExporter = builder.AddContainer("node-exporter", "prom/node-exporter")
    .WithHttpEndpoint(targetPort: 9100, name: "http");

var cadvisor = builder.AddContainer("cadvisor", "gcr.io/cadvisor/cadvisor")
    .WithHttpEndpoint(targetPort: 8080, name: "http");

var granafa = builder.AddContainer("grafana", "grafana/grafana")
                    .WithHttpEndpoint(port: 5300, targetPort: 3000, name: "http")
                    .WithBindMount("../../infra/grafana/config", "/etc/grafana")
                    .WithBindMount("../../infra/grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
                    .WithEnvironment("GF_PATHS_CONFIG", "/etc/grafana/grafana.ini")
                    .WithEnvironment("GF_INSTALL_PLUGINS", "grafana-clock-panel,grafana-piechart-panel")
                    .WithEnvironment("GF_INSTALL_PLUGINS", "https://storage.googleapis.com/integration-artifacts/grafana-lokiexplore-app/grafana-lokiexplore-app-latest.zip;grafana-lokiexplore-app")
                    .WithEnvironment("PROMETHEUS_ENDPOINT", prometheus.GetEndpoint("http"))
                    .WithEnvironment("GF_PATHS_PROVISIONING", "/etc/grafana/provisioning");

builder.AddOpenTelemetryCollector("otelcollector", "../../infra/otelcollector/config.yaml")
       .WithEnvironment("PROMETHEUS_ENDPOINT", $"{prometheus.GetEndpoint("http")}/api/v1/otlp")
       .WithEnvironment("LOKI_ENDPOINT", $"{loki.GetEndpoint("http")}/loki/api/v1/push");



var bestiaryArenaCrackerSql = builder
    .AddSqlServer("bestiary-arena-cracker-sql", port: 56715)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .PublishAsConnectionString()
    .AddDatabase("BestiaryArenaCracker");

var BestiaryArenaCrackerRedis = builder
    .AddRedis("BestiaryArenaCrackerRedis", 56716)
    .WithRedisInsight(configureContainer: (options) =>
    {
        options.WithHostPort(56717);
    });

builder
    .AddProject<Projects.BestiaryArenaCracker_MigrationService>("bestiaryarenacracker-migrationservice")
    .WithReference(bestiaryArenaCrackerSql)
    .WaitFor(bestiaryArenaCrackerSql);

var bestiaryArenaCrackerApi = builder
    .AddProject<Projects.BestiaryArenaCracker_Api>("bestiaryarenacracker-api")
    .WithReference(bestiaryArenaCrackerSql)
    .WithReference(BestiaryArenaCrackerRedis)
    .WaitFor(bestiaryArenaCrackerSql)
    .WaitFor(BestiaryArenaCrackerRedis)
    .WithHttpHealthCheck("/health");


builder.
    AddNpmApp("bestiary-arena-cracker-ui", "../BestiaryArenaCracker.UI", "dev")
    .WithReference(bestiaryArenaCrackerApi)
    .WithHttpEndpoint(env: "PORT", port: 3000)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
