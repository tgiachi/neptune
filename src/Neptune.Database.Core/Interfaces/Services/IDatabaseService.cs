namespace Neptune.Database.Core.Interfaces.Services;

public interface IDatabaseService
{
    Task StartAsync();

    Task StopAsync();
}
