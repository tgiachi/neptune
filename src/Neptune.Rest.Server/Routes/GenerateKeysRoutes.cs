using Neptune.Server.Core.Data.Cryptography;
using Neptune.Server.Core.Data.Rest;
using Neptune.Server.Core.Data.Rest.Base;

namespace Neptune.Rest.Server.Routes;

public static class GenerateKeysRoutes
{
    public static IEndpointRouteBuilder MapGenerateKeysRoutes(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/keys");

        group.MapPost(
                "",
                () =>
                {
                    var service = new NeptuneCryptographyService();

                    var keys = service.CreateWithNewKeyPair();


                    return RestResultObject<KeyResponseObject>.CreateSuccess(
                        new KeyResponseObject(keys.ExportPublicKeyBase64(), keys.ExportPrivateKeyBase64())
                    );
                }
            )
            .WithDescription("Generate a new key pair")
            .WithName("GenerateKeyPair")
            .Produces<KeyResponseObject>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);


        return endpoints;
    }
}
