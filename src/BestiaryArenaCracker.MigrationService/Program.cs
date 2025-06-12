using BestiaryArenaCracker.Repository.Context;

namespace BestiaryArenaCracker.MigrationService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddHostedService<RoomThumbnailsDownload>();
        builder.Services.AddHostedService<Worker>();

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName))
            .WithTracing(tracing => tracing.AddSource(RoomThumbnailsDownload.ActivitySourceName));

        builder.AddSqlServerDbContext<ApplicationDbContext>("BestiaryArenaCracker");

        var host = builder.Build();
        host.Run();
    }
}