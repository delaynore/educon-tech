using Microsoft.AspNetCore.Routing;

namespace EducationContentService.UseCases;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder builder);
}
