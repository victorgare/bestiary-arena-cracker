using BestiaryArenaCracker.Repository.Context;

namespace BestiaryArenaCracker.MigrationService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddHostedService<Worker>();

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

        builder.AddSqlServerDbContext<ApplicationDbContext>("BestiaryArenaCracker");

        var host = builder.Build();
        host.Run();
    }
}