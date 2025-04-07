using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Neptune.Packets.Messages;
using Neptune.Transport.Core.Events;
using Neptune.Transport.Core.Interfaces;

namespace Neptune.Transport.Core.Base;

/// <summary>
/// Default implementation of the transport manager
/// </summary>
public class TransportManager : ITransportManager
{
    private readonly ILogger<TransportManager> _logger;
    private readonly ConcurrentDictionary<string, ITransport> _transports = new ConcurrentDictionary<string, ITransport>();
    private readonly ITransportRoutingStrategy _routingStrategy;
    private bool _disposed;

    public IReadOnlyDictionary<string, ITransport> Transports => _transports;

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    public TransportManager(ILogger<TransportManager> logger, ITransportRoutingStrategy routingStrategy = null)
    {
        _logger = logger;
        _routingStrategy = routingStrategy ?? new DefaultTransportRoutingStrategy(logger);
    }

    public void AddTransport(ITransport transport)
    {
        if (transport == null)
            throw new ArgumentNullException(nameof(transport));

        if (_transports.TryAdd(transport.TransportId, transport))
        {
            transport.MessageReceived += Transport_MessageReceived;
            _logger.LogInformation(
                "Added transport: {TransportId} of type {TransportType}",
                transport.TransportId,
                transport.TransportType
            );
        }
        else
        {
            _logger.LogWarning("Transport with ID {TransportId} already exists", transport.TransportId);
        }
    }

    public bool RemoveTransport(string transportId)
    {
        if (_transports.TryRemove(transportId, out var transport))
        {
            transport.MessageReceived -= Transport_MessageReceived;
            _logger.LogInformation("Removed transport: {TransportId}", transportId);
            return true;
        }

        return false;
    }

    public ITransport GetTransport(string transportId)
    {
        _transports.TryGetValue(transportId, out var transport);
        return transport;
    }

    public async Task StartAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting all transports...");

        var tasks = _transports.Values.Select(t => StartTransportSafelyAsync(t, cancellationToken));
        await Task.WhenAll(tasks);

        _logger.LogInformation("All transports started");
    }

    public async Task StopAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping all transports...");

        var tasks = _transports.Values.Select(t => StopTransportSafelyAsync(t, cancellationToken));
        await Task.WhenAll(tasks);

        _logger.LogInformation("All transports stopped");
    }

    public async Task<bool> SendMessageAsync(NeptuneMessage message, CancellationToken cancellationToken = default)
    {
        if (_transports.Count == 0)
        {
            _logger.LogWarning("No transports available to send message");
            return false;
        }

        var transport = _routingStrategy.SelectTransport(message, _transports.Values);
        if (transport == null)
        {
            _logger.LogWarning("No suitable transport found for message");
            return false;
        }

        return await transport.SendMessageAsync(message, cancellationToken);
    }

    private async Task StartTransportSafelyAsync(ITransport transport, CancellationToken cancellationToken)
    {
        try
        {
            await transport.StartAsync(cancellationToken);
            _logger.LogInformation("Started transport: {TransportId}", transport.TransportId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start transport {TransportId}", transport.TransportId);
        }
    }

    private async Task StopTransportSafelyAsync(ITransport transport, CancellationToken cancellationToken)
    {
        try
        {
            await transport.StopAsync(cancellationToken);
            _logger.LogInformation("Stopped transport: {TransportId}", transport.TransportId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping transport {TransportId}", transport.TransportId);
        }
    }

    private void Transport_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        MessageReceived?.Invoke(this, e);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            foreach (var transport in _transports.Values)
            {
                transport.MessageReceived -= Transport_MessageReceived;
                transport.Dispose();
            }

            _transports.Clear();
            _disposed = true;
        }
    }
}
