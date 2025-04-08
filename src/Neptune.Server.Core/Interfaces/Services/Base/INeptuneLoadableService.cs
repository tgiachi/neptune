namespace Neptune.Server.Core.Interfaces.Services.Base;

public interface INeptuneLoadableService
{
    Task StartAsync();

    Task StopAsync();
}
