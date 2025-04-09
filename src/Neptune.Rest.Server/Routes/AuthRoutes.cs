using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Neptune.Rest.Server.Extensions;
using Neptune.Rest.Server.Interfaces;
using Neptune.Server.Core.Data.Rest;
using Neptune.Server.Core.Data.Rest.Base;
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

                if (result.IsSuccess)
                {
                    return RestResultObject<LoginResponseObject>.CreateSuccess(result).ToResult();
                }

                return RestResultObject<LoginResponseObject>.CreateError(result.Message).ToResult();
            }
        );

        group.MapPost(
                "/register",
                async (
                    RegisterRequestObject request, IAuthService loginService
                ) =>
                {
                    var result = await loginService.RegisterAsync(request);

                    if (result.IsSuccess)
                    {
                        return RestResultObject<RegisterResponseObject>.CreateSuccess(result).ToResult();
                    }

                    return RestResultObject<RegisterResponseObject>.CreateError(result.Message).ToResult();
                }
            )
            .AddFluentValidationAutoValidation();


        group.AllowAnonymous().ProducesValidationProblem();


        return endpoints;
    }
}
