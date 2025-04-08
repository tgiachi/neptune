using Neptune.Server.Core.Data.Config;
using Neptune.Server.Core.Interfaces.Services;

namespace Neptune.Rest.Server.Services;

public class RabbitMqMessageQueueService : IMessageQueueService
{
    private readonly NeptuneServerConfig _neptuneServerConfig;

    private readonly ILogger _logger;

    public RabbitMqMessageQueueService(ILogger<RabbitMqMessageQueueService> logger, NeptuneServerConfig neptuneServerConfig)
    {
        _neptuneServerConfig = neptuneServerConfig;
        _logger = logger;
    }

    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }
}
