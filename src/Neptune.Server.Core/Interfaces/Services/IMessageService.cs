using Neptune.Server.Core.Interfaces.Services.Base;

namespace Neptune.Server.Core.Interfaces.Services;

public interface IMessageService : INeptuneLoadableService
{
    Task DispatchMessageAsync(string from, string to, string message);

}
