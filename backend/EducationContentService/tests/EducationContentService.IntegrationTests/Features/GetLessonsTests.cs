using System.Globalization;
using EducationContentService.Contracts.Lessons;
using EducationContentService.Domain.Lessons;
using EducationContentService.Domain.ValueObjects;
using EducationContentService.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace EducationContentService.IntegrationTests.Features;

public sealed class GetLessonsTests : EducationContentServiceTestsBase
{
    public GetLessonsTests(IntegrationTestsWebFactory webFactory)
        : base(webFactory)
    {
    }

    [Fact]
    public async Task GetLessons_Should_Return_Lessons()
    {
        var cancellationToken = CancellationToken.None;
        var lessons = Enumerable.Range(1, 3)
            .Select(index => new Lesson(
                Guid.NewGuid(),
                Title.Create($"Lesson {index}").Value,
                Description.Create($"Description {index}").Value,
                Guid.NewGuid()))
            .ToArray();

        await ExecuteInDbContext(cancellationToken, (db, _, _) =>
        {
            db.Lessons.AddRange(lessons);
            return db.SaveChangesAsync(cancellationToken);
        });

        var request = new GetLessonsRequest(null, 1, 3);

        var queryParams = new Dictionary<string, string?>
        {
            {
                "page", request.Page.ToString(CultureInfo.InvariantCulture)
            },
            {
                "pageSize", request.PageSize.ToString(CultureInfo.InvariantCulture)
            },
        };
        var url = QueryHelpers.AddQueryString("api/lessons", queryParams);

        var response = await AppHttpClient.GetAsync(url, cancellationToken);

        var responseResult = await response.HandleResponseAsync<PaginatedLessonsDto>(cancellationToken);

        Assert.True(responseResult.IsSuccess);
        Assert.Equal(3, responseResult.Value.Items.Count);
        foreach (var lesson in lessons)
        {
            var result = Assert.Single(responseResult.Value.Items, item => item.Id == lesson.Id);
            Assert.NotNull(result.Media);
            Assert.Equal(lesson.VideoId, result.Media.Id);
            Assert.Equal($"url/{lesson.VideoId}", result.Media.Url);
        }
    }
}
