using EducationContentService.Infrastructure.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace EducationContentService.IntegrationTests.Infrastructure;

public abstract class EducationContentServiceTestsBase : IClassFixture<IntegrationTestsWebFactory>
{
    protected EducationContentServiceTestsBase(IntegrationTestsWebFactory factory)
    {
        AppHttpClient = factory.CreateClient();
        HttpClient = new HttpClient();
        Services = factory.Services;
    }

    protected IServiceProvider Services { get; private set; }

    protected HttpClient AppHttpClient { get; private set; }

    protected HttpClient HttpClient { get; private set; }

    protected async Task ExecuteInDbContext(
        CancellationToken cancellationToken,
        params Func<EducationDbContext, IServiceProvider, CancellationToken, Task>[] actions)
    {
        await using var serviceScope = Services.CreateAsyncScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<EducationDbContext>();

        foreach (var action in actions)
        {
            await action(dbContext, serviceScope.ServiceProvider, cancellationToken);
        }
    }
}
