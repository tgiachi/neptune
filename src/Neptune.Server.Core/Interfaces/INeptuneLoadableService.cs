namespace Neptune.Server.Core.Interfaces;

public interface INeptuneLoadableService
{
    Task StartAsync();

    Task StopAsync();
}
