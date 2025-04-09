using Neptune.Server.Core.Interfaces.Services.Base;

namespace Neptune.Server.Core.Interfaces.Services;

public interface IMessageService : INeptuneLoadableService
{
    Task<Guid> DispatchMessageAsync(string from, string to, string message);

}
