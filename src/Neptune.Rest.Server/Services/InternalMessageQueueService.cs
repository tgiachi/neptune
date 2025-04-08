using Neptune.Server.Core.Interfaces.Services;

namespace Neptune.Rest.Server.Services;

public class InternalMessageQueueService : IMessageQueueService
{
    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }
}
