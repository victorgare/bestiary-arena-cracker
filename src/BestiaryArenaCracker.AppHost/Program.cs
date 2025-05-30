var builder = DistributedApplication.CreateBuilder(args);

var bestiaryArenaCrackerSql = builder
    .AddSqlServer("tibia-soul-sql")
    .WithDataVolume()
    .PublishAsConnectionString()
    .AddDatabase("BestiaryArenaCracker");

builder
    .AddProject<Projects.BestiaryArenaCracker_Api>("bestiaryarenacracker-api")
    .WithReference(bestiaryArenaCrackerSql);

builder.Build().Run();
