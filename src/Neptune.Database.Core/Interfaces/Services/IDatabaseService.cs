using Neptune.Server.Core.Interfaces;
using Neptune.Server.Core.Interfaces.Services;
using Neptune.Server.Core.Interfaces.Services.Base;

namespace Neptune.Database.Core.Interfaces.Services;

public interface IDatabaseService : INeptuneLoadableService
{
    Task StartAsync();

    Task StopAsync();
}
