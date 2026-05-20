using Be.BlogManagementAssignment.Application.Extentions;

namespace Be.BlogManagementAssignment.Api.Extentions;

public static class EndPointExtention
{
    public static void MapEndPoints<T>(this RouteGroupBuilder group) where T : IMinimalEndPoint, new()
    {
        var endPoint = new T();
        endPoint.AddRoutes(group);
    }
}
