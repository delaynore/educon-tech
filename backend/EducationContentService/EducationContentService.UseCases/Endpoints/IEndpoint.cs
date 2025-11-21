using Microsoft.AspNetCore.Routing;

namespace EducationContentService.UseCases.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder builder);
}
