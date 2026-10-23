using System.Text.Json.Serialization;
using EducationContentService.Infrastructure.Postgres;
using EducationContentService.IntegrationTests.Mocks;
using FileService.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace EducationContentService.IntegrationTests.Infrastructure;

public sealed class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres")
        .WithDatabase("education_service_db_tests")
        .WithUsername(PostgreSqlBuilder.DefaultUsername)
        .WithPassword(PostgreSqlBuilder.DefaultPassword)
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        await using var scope = Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<EducationDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.UseSetting("ConnectionStrings:Database", _dbContainer.GetConnectionString());
        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile(
                Path.Combine(
                    AppContext.BaseDirectory,
                    $"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json"),
                optional: true);
        });

        builder.ConfigureTestServices(services =>
        {
            services.ConfigureHttpJsonOptions(jsonOptions =>
            {
                jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.RemoveAll<EducationDbContext>();
            services.AddDbContextPool<EducationDbContext>((_, dbBuilder) =>
            {
                dbBuilder.UseNpgsql(_dbContainer.GetConnectionString());
            });

            MockFileServiceHttpClient(services);
        });
    }

    private static void MockFileServiceHttpClient(IServiceCollection services)
    {
        services.RemoveAll<IFileServiceHttpClient>();
        services.AddScoped<IFileServiceHttpClient, FileServiceHttpClientMock>();
    }
}
