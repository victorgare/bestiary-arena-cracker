var builder = DistributedApplication.CreateBuilder(args);

var bestiaryArenaCrackerSql = builder
    .AddSqlServer("bestiary-arena-cracker-sql", port: 56715)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .PublishAsConnectionString()
    .AddDatabase("BestiaryArenaCracker");

builder
    .AddProject<Projects.BestiaryArenaCracker_MigrationService>("bestiaryarenacracker-migrationservice")
    .WithReference(bestiaryArenaCrackerSql)
    .WaitFor(bestiaryArenaCrackerSql);

var bestiaryArenaCrackerApi = builder
    .AddProject<Projects.BestiaryArenaCracker_Api>("bestiaryarenacracker-api")
    .WithReference(bestiaryArenaCrackerSql)
    .WaitFor(bestiaryArenaCrackerSql);


builder.
    AddNpmApp("bestiary-arena-cracker-ui", "../BestiaryArenaCracker.UI", "dev")
    .WithReference(bestiaryArenaCrackerApi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
