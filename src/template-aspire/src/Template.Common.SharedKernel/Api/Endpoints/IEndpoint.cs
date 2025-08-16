using Microsoft.AspNetCore.Routing;

namespace Template.Common.SharedKernel.Api.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
