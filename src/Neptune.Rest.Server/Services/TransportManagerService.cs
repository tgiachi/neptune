using AbyssIrc.Signals.Interfaces.Listeners;
using AbyssIrc.Signals.Interfaces.Services;
using Neptune.Server.Core.Data.Config;
using Neptune.Server.Core.Events.Messages;
using Neptune.Server.Core.Interfaces.Services;
using Neptune.Transport.Core.Interfaces;

namespace Neptune.Rest.Server.Services;

public class TransportManagerService : ITransportManagerService, IAbyssSignalListener<MessageAddedEvent>
{
    private readonly ILogger _logger;


    private readonly IAbyssSignalService _abyssSignalService;

    private readonly Dictionary<string, INeptuneTransport> _transports = new();

    private readonly IServiceProvider _serviceProvider;

    private readonly NeptuneServerConfig _neptuneServerConfig;

    public TransportManagerService(
        ILogger<TransportManagerService> logger, IServiceProvider serviceProvider, NeptuneServerConfig neptuneServerConfig,
        IAbyssSignalService abyssSignalService
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _neptuneServerConfig = neptuneServerConfig;
        _abyssSignalService = abyssSignalService;

        abyssSignalService.Subscribe(this);

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

    public async Task OnEventAsync(MessageAddedEvent signalEvent)
    {
        _logger.LogInformation("Sending message to transports: {Message}", signalEvent.Message);
        foreach (var (_, transport) in _transports)
        {
            try
            {
                await transport.SendMessageAsync(signalEvent.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while sending message to transport {Transport}", transport.Name);
            }
        }
    }
}
