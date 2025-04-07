using Neptune.Packets.Messages;
using Neptune.Transport.Core.Events;

namespace Neptune.Transport.Core.Interfaces;

/// <summary>
/// Manages multiple transports for the Anaconda protocol
/// </summary>
public interface ITransportManager : IDisposable
{
    /// <summary>
    /// Gets all registered transports
    /// </summary>
    IReadOnlyDictionary<string, ITransport> Transports { get; }

    /// <summary>
    /// Adds a transport to the manager
    /// </summary>
    void AddTransport(ITransport transport);

    /// <summary>
    /// Removes a transport from the manager
    /// </summary>
    bool RemoveTransport(string transportId);

    /// <summary>
    /// Gets a transport by ID
    /// </summary>
    ITransport GetTransport(string transportId);

    /// <summary>
    /// Starts all transports
    /// </summary>
    Task StartAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops all transports
    /// </summary>
    Task StopAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a message through the most appropriate transport
    /// </summary>
    Task<bool> SendMessageAsync(NeptuneMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Event that fires when a message is received on any transport
    /// </summary>
    event EventHandler<MessageReceivedEventArgs> MessageReceived;
}
