using Microsoft.AspNetCore.Mvc;

using Neptune.Server.Core.Data.Rest;

namespace Neptune.Rest.Server.Routes;

public static class MessagesRoutes
{
    public static IEndpointRouteBuilder MapMessageRoutes(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/messages");


        group.MapPost(
                "/send",
                ([FromBody] MessageRequestObject messageRequest) =>
                {
                    return Results.Ok(new MessageResponseObject());
                }

            )
            .WithDescription("Send a message")
            .WithName("SendMessage")
            .Produces<MessageResponseObject>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();


        return endpoints;
    }
}
