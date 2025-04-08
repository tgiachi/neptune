using Neptune.Server.Core.Data.Config;
using Neptune.Server.Core.Data.Rest;

namespace Neptune.Rest.Server.Routes;

public static class SystemRoutes
{
    public static IEndpointRouteBuilder MapSystemRoutes(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/system");

        group.MapGet(
                "/info",
                (NeptuneServerConfig serverConfig) =>
                    Results.Ok(new SystemInfoObject(serverConfig.NodeName, serverConfig.NodeId))
            )
            .WithDescription("Get system information")
            .WithName("SystemInfo")
            .Produces<SystemInfoObject>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);


        return endpoints;
    }
}
