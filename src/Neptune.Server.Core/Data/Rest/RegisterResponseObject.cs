namespace Neptune.Server.Core.Data.Rest;

public class RegisterResponseObject
{

    public string FullNodeId { get; set; }

    public string PrivateKey { get; set; }

    public string PublicKey { get; set; }

    public LoginResponseObject LoginResponse { get; set; }

    public bool IsSuccess { get; set; }

    public string? Message { get; set; }

}
