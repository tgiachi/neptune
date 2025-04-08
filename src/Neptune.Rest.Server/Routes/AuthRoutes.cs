using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Neptune.Rest.Server.Interfaces;
using Neptune.Server.Core.Data.Rest;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Neptune.Rest.Server.Routes;

public static class AuthRoutes
{
    public static IEndpointRouteBuilder MapAuthRoutes(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth");


        group.MapPost(
            "/login",
            async (LoginRequestObject request, IAuthService loginService) =>
            {
                var result = await loginService.LoginAsync(request);

                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            }
        );

        group.MapPost(
                "/register",
                async (
                    RegisterRequestObject request, IAuthService loginService
                ) =>
                {
                    var result = await loginService.RegisterAsync(request);

                    return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
                }
            )
            .AddFluentValidationAutoValidation();


        group.AllowAnonymous().ProducesValidationProblem();


        return endpoints;
    }
}
