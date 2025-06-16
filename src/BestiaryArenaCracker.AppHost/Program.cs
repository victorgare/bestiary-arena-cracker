var builder = DistributedApplication.CreateBuilder(args);

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
