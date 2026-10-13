using System.Text.Json.Serialization;
using FileService.Infrastructure.Postgres;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;

namespace FileService.IntegrationTests.Infrastructure;

public sealed class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres")
        .WithDatabase("file_service_db_tests")
        .WithUsername(PostgreSqlBuilder.DefaultUsername)
        .WithPassword(PostgreSqlBuilder.DefaultPassword)
        .Build();

    private readonly MinioContainer _s3Container = new MinioBuilder("minio/minio")
        .WithUsername("minioadmin")
        .WithPassword("minioadmin")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _s3Container.StartAsync();

        await using var scope = Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<FileServiceDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await _s3Container.StopAsync();
        await _s3Container.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile(
                Path.Combine(
                    AppContext.BaseDirectory,
                    $"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json"),
                optional: false);
            config.AddInMemoryCollection([
                KeyValuePair.Create<string, string?>("ConnectionStrings:Database", _dbContainer.GetConnectionString()),
                KeyValuePair.Create<string, string?>("FileStorageOptions:Endpoint", _s3Container.GetConnectionString()),
                KeyValuePair.Create<string, string?>("FileStorageOptions:AccessKey", _s3Container.GetAccessKey()),
                KeyValuePair.Create<string, string?>("FileStorageOptions:SecretKey", _s3Container.GetSecretKey()),
            ]);
        });

        builder.ConfigureTestServices(services =>
        {
            services.ConfigureHttpJsonOptions(jsonOptions =>
            {
                jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.RemoveAll<FileServiceDbContext>();
            services.AddDbContextPool<FileServiceDbContext>((_, dbBuilder) =>
            {
                dbBuilder.UseNpgsql(_dbContainer.GetConnectionString());
            });

            // services.RemoveAll<IAmazonS3>();
            //
            // services.AddSingleton<IAmazonS3>(sp =>
            // {
            //     var options = sp.GetRequiredService<IOptions<S3Options>>().Value;
            //
            //     var config = new AmazonS3Config
            //     {
            //         ServiceURL = options.Endpoint,
            //         ForcePathStyle = true,
            //         UseHttp = !options.WithSsl,
            //     };
            //
            //     return new AmazonS3Client(options.AccessKey, options.SecretKey, config);
            // });
        });
    }
}
