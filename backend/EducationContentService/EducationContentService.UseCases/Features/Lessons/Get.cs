using Core.Validation;
using CSharpFunctionalExtensions;
using EducationContentService.Contracts.Lessons;
using EducationContentService.UseCases.Database;
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

    public GetHandler(
        IEducationReadDbContext readDbContext,
        IValidator<GetLessonRequest> validator)
    {
        _readDbContext = readDbContext;
        _validator = validator;
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
                l.CreatedAtUtc,
                l.UpdatedAtUtc))
            .Skip(request.PageSize * (request.Page - 1))
            .Take(request.PageSize)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return new PaginatedLessonsDto(lessons, count);
    }
}
