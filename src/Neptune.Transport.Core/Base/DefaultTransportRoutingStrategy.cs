using Microsoft.Extensions.Logging;
using Neptune.Packets.Messages;
using Neptune.Transport.Core.Interfaces;

namespace Neptune.Transport.Core.Base;

/// <summary>
/// Default implementation of transport routing strategy
/// </summary>
public class DefaultTransportRoutingStrategy : ITransportRoutingStrategy
{
    private readonly ILogger _logger;

    public DefaultTransportRoutingStrategy(ILogger logger)
    {
        _logger = logger;
    }

    public ITransport SelectTransport(NeptuneMessage message, IEnumerable<ITransport> availableTransports)
    {
        // Simple strategy: select the first active transport
        var activeTransports = availableTransports.Where(t => t.IsActive).ToList();

        if (activeTransports.Count == 0)
        {
            _logger.LogWarning("No active transports available");
            return null;
        }


        if (message.Header.Type == Neptune.Packets.Types.MessageType.PING ||
            message.Header.Type == Neptune.Packets.Types.MessageType.PONG ||
            message.Header.Type == Neptune.Packets.Types.MessageType.ERROR)
        {
            return activeTransports.OrderBy(t => t.Metrics.AverageRoundTripTimeMs)
                .FirstOrDefault(t => t.Metrics.AverageRoundTripTimeMs > 0) ?? activeTransports.First();
        }

        // Default to first active transport
        return activeTransports.First();
    }
}
