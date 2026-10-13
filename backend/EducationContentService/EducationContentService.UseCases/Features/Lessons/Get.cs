using Core.Validation;
using CSharpFunctionalExtensions;
using EducationContentService.Contracts.Lessons;
using EducationContentService.UseCases.Database;
using FileService.Contracts;
using FileService.Contracts.Dto;
using FluentValidation;
using Framework.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace EducationContentService.UseCases.Features.Lessons;

public sealed class GetLessonRequestValidator : AbstractValidator<GetLessonRequest>
{
    public GetLessonRequestValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(1000)
            .WithError(GeneralErrors.ValueIsInvalid("search"));

        RuleFor(x => x.Page)
            .NotNull().WithError(GeneralErrors.ValueIsInvalid("page"))
            .GreaterThan(0).WithError(GeneralErrors.ValueIsInvalid("page"));

        RuleFor(x => x.PageSize)
            .NotNull().WithError(GeneralErrors.ValueIsInvalid("pageSize"))
            .GreaterThan(0).WithError(GeneralErrors.ValueIsInvalid("pageSize"));
    }
}

public sealed class GetEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "/lessons",
            async Task<EndpointResult<PaginatedLessonsDto>>(
                [AsParameters] GetLessonRequest request,
                [FromServices] GetHandler handler,
                CancellationToken cancellationToken) => await handler.Handle(request, cancellationToken));
    }
}

public sealed class GetHandler
{
    private readonly IEducationReadDbContext _readDbContext;
    private readonly IValidator<GetLessonRequest> _validator;
    private readonly IFileServiceHttpClient _fileServiceHttpClient;

    public GetHandler(
        IEducationReadDbContext readDbContext,
        IValidator<GetLessonRequest> validator,
        IFileServiceHttpClient fileServiceHttpClient)
    {
        _readDbContext = readDbContext;
        _validator = validator;
        _fileServiceHttpClient = fileServiceHttpClient;
    }

    public async Task<Result<PaginatedLessonsDto, Error>> Handle(
        GetLessonRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        var query = _readDbContext.LessonQuery;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(l =>
                EF.Functions.Like(l.Title.Value.ToLower(), $"%{request.Search.ToLower()}%"));
        }

        var count = await query.CountAsync(cancellationToken);

        var lessons = await query
            .Select(l => new LessonDto(
                l.Id,
                l.Title.Value,
                l.Description.Value,
                l.VideoId.HasValue ? new MediaDto(l.VideoId.Value, string.Empty, string.Empty) : null,
                l.CreatedAtUtc,
                l.UpdatedAtUtc))
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip(request.PageSize * (request.Page - 1))
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var ids = lessons
            .Where(x => x.Media is not null)
            .Select(x => x.Media!.Id)
            .ToList()
            .AsReadOnly();

        var getMediaAssetsRequest = new GetMediaAssetsRequest(ids);
        var mediaAssets = await _fileServiceHttpClient
            .GetMediaAssets(getMediaAssetsRequest, cancellationToken);

        if (mediaAssets.IsFailure)
        {
            return mediaAssets.Error;
        }

        var mediaAssetDict = mediaAssets.Value.MediaAssets
            .ToDictionary(x => x.MediaAssetId, x => x);

        foreach (var lesson in lessons)
        {
            if (lesson.Media is not null
                && mediaAssetDict.TryGetValue(lesson.Id, out var mediaAssetDto))
            {
                lesson.Media = new MediaDto(
                    mediaAssetDto.MediaAssetId,
                    mediaAssetDto.Url,
                    mediaAssetDto.Status);
            }
        }

        return new PaginatedLessonsDto(lessons, count);
    }
}
