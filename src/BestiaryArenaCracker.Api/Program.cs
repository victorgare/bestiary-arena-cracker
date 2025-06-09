
using BestiaryArenaCracker.Api.Infrastructure.DependencyInjection;
using BestiaryArenaCracker.Repository.Context;

namespace BestiaryArenaCracker.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // add swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // add dependency
        builder.AddSqlServerDbContext<ApplicationDbContext>("BestiaryArenaCracker");

        builder.Services.AddProvidersDependency();
        builder.Services.AddServicesDependency();
        builder.Services.AddRepositoriesDependency();

        // enable CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("https://bestiaryarena.com")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();
        app.UseCors();

        app.Run();
    }
}
