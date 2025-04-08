namespace Neptune.Rest.Server.Routes;

public static class MessagesRoutes
{



    public static IEndpointRouteBuilder MapMessageRoutes(this IEndpointRouteBuilder endpoints)
    {

        var group = endpoints.MapGroup("/messages");



        return endpoints;
    }

}
