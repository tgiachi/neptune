using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Neptune.Rest.Server.Extensions;
using Neptune.Rest.Server.Interfaces;
using Neptune.Server.Core.Data.Rest;
using Neptune.Server.Core.Data.Rest.Base;
using Neptune.Server.Core.Interfaces.Services;

namespace Neptune.Rest.Server.Routes;

public static class MessagesRoutes
{
    public static IEndpointRouteBuilder MapMessageRoutes(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/messages");


        group.MapPost(
                "/send",
                async (
                    HttpContext httpContext, [FromBody] MessageRequestObject messageRequest, IMessageService messageService
                ) =>
                {
                    var user = httpContext.User;

                    var fullName = user.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                    await messageService.DispatchMessageAsync(fullName, messageRequest.To, messageRequest.Message);

                    return RestResultObject<MessageResponseObject>.CreateSuccess(new MessageResponseObject()).ToResult();
                }
            )
            .WithDescription("Send a message")
            .WithName("SendMessage")
            .Produces<RestResultObject<MessageResponseObject>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();


        return endpoints;
    }
}
