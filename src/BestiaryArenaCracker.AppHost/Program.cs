using BestiaryArenaCracker.AppHost.OpenTelemetryCollector;

var builder = DistributedApplication.CreateBuilder(args);

var prometheus = builder
    .AddContainer("prometheus", "prom/prometheus", "v3.2.1")
    .WithBindMount("../../prometheus", "/etc/prometheus", isReadOnly: true)
    .WithArgs("--web.enable-otlp-receiver", "--config.file=/etc/prometheus/prometheus.yml")
    .WithHttpEndpoint(targetPort: 9090, name: "http");

var grafana = builder
    .AddContainer("grafana", "grafana/grafana")
    .WithBindMount("../../grafana/config", "/etc/grafana", isReadOnly: true)
    .WithBindMount("../../grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
    .WithEnvironment("PROMETHEUS_ENDPOINT", prometheus.GetEndpoint("http"))
    .WithHttpEndpoint(targetPort: 3000, name: "http");

builder.AddOpenTelemetryCollector("otelcollector", "../../otelcollector/config.yaml")
       .WithEnvironment("PROMETHEUS_ENDPOINT", $"{prometheus.GetEndpoint("http")}/api/v1/otlp");



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
    .WaitFor(BestiaryArenaCrackerRedis);


builder.
    AddNpmApp("bestiary-arena-cracker-ui", "../BestiaryArenaCracker.UI", "dev")
    .WithReference(bestiaryArenaCrackerApi)
    .WithHttpEndpoint(env: "PORT", port: 3000)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
