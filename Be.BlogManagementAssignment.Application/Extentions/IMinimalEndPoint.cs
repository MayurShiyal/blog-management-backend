using Microsoft.AspNetCore.Routing;

namespace Be.BlogManagementAssignment.Application.Extentions;

public interface IMinimalEndPoint
{
    void AddRoutes(IEndpointRouteBuilder app);
}
