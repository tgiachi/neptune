using AbyssIrc.Signals.Interfaces.Services;
using Neptune.Packets.Messages;
using Neptune.Server.Core.Interfaces.Services;

namespace Neptune.Rest.Server.Services;

public class InternalMessageQueueService : IMessageQueueService
{
    private readonly IAbyssSignalService _abyssSignalService;
    private readonly ILogger _logger;

    public InternalMessageQueueService(ILogger<InternalMessageQueueService> logger, IAbyssSignalService abyssSignalService)
    {
        _abyssSignalService = abyssSignalService;
        _logger = logger;
    }

    public Task StartAsync()
    {
        _logger.LogInformation("Internal message queue service started.");
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }

    public Task PublishAsync(NeptuneMessage message)
    {
        return Task.CompletedTask;
    }
}
