using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Neptune.Rest.Server.Interfaces;
using Neptune.Server.Core.Data.Rest;

namespace Neptune.Rest.Server.Routes;

public static class MessagesRoutes
{
    public static IEndpointRouteBuilder MapMessageRoutes(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/messages");


        group.MapPost(
                "/send",
                (HttpContext httpContext, [FromBody] MessageRequestObject messageRequest, IAuthService authService) =>
                {

                    var user = httpContext.User;

                    var fullName = user.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

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
