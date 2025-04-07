using Neptune.Server.Core.Interfaces;

namespace Neptune.Database.Core.Interfaces.Services;

public interface IDatabaseService : INeptuneLoadableService
{
    Task StartAsync();

    Task StopAsync();
}
