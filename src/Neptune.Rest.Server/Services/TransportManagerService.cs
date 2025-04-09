using Neptune.Server.Core.Data.Config;
using Neptune.Server.Core.Interfaces.Services;
using Neptune.Transport.Core.Interfaces;

namespace Neptune.Rest.Server.Services;

public class TransportManagerService : ITransportManagerService
{
    private readonly ILogger _logger;

    private readonly Dictionary<string, INeptuneTransport> _transports = new();

    private readonly IServiceProvider _serviceProvider;

    private readonly NeptuneServerConfig _neptuneServerConfig;

    public TransportManagerService(
        ILogger<TransportManagerService> logger, IServiceProvider serviceProvider, NeptuneServerConfig neptuneServerConfig
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _neptuneServerConfig = neptuneServerConfig;

        InitializeTransports();
    }

    private void InitializeTransports()
    {
        var transportTypes = _serviceProvider.GetServices<INeptuneTransport>();

        foreach (var transport in transportTypes)
        {
            if (_neptuneServerConfig.Transports.Names.Contains(transport.Name.ToLower()))
            {
                _transports.Add(transport.Name, transport);
                _logger.LogInformation("Transport {Name} initialized.", transport.Name);
            }
        }
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
