using Neptune.Server.Core.Data.Rest;

namespace Neptune.Rest.Server.Interfaces;

public interface IAuthService
{
    Task<LoginResponseObject> LoginAsync(LoginRequestObject loginRequest);


    Task<RegisterResponseObject> RegisterAsync(RegisterRequestObject registerRequest);
}
