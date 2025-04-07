using Neptune.Packets.Messages;

namespace Neptune.Transport.Core.Interfaces;

/// <summary>
/// Interface for transport routing strategies
/// </summary>
public interface ITransportRoutingStrategy
{
    /// <summary>
    /// Selects the most appropriate transport for a message
    /// </summary>
    ITransport SelectTransport(NeptuneMessage message, IEnumerable<ITransport> availableTransports);
}
