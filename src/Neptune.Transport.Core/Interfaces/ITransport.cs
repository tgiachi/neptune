using Neptune.Packets.Messages;
using Neptune.Transport.Core.Events;
using Neptune.Transport.Core.Models;

namespace Neptune.Transport.Core.Interfaces;

/// <summary>
/// Represents transport layer for Anaconda protocol
/// </summary>
public interface ITransport : IDisposable
{
    /// <summary>
    /// Gets the unique identifier for this transport instance
    /// </summary>
    string TransportId { get; }

    /// <summary>
    /// Gets the transport type
    /// </summary>
    string TransportType { get; }

    /// <summary>
    /// Gets whether the transport is active
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Gets current transport metrics
    /// </summary>
    TransportMetrics Metrics { get; }

    /// <summary>
    /// Starts the transport
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the transport
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a message through this transport
    /// </summary>
    Task<bool> SendMessageAsync(NeptuneMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Discovers peers on this transport
    /// </summary>
    Task<IEnumerable<PeerInfo>> DiscoverPeersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Event that fires when a message is received
    /// </summary>
    event EventHandler<MessageReceivedEventArgs> MessageReceived;

    /// <summary>
    /// Event that fires when a peer is discovered
    /// </summary>
    event EventHandler<PeerDiscoveredEventArgs> PeerDiscovered;

    /// <summary>
    /// Event that fires when the transport status changes
    /// </summary>
    event EventHandler<TransportStatusChangedEventArgs> StatusChanged;
}
