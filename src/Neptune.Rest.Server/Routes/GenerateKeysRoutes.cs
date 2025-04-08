using Neptune.Server.Core.Data.Cryptography;
using Neptune.Server.Core.Data.Rest;

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
                    var keys = new NeptuneEncryptorX25519();

                    keys.Generate();


                    return Results.Ok(new KeyResponseObject(keys.ExportPublicKeyBase64(), keys.ExportPrivateKeyBase64()));
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
