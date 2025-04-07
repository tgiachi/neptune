using Neptune.Transport.Core.Interfaces;

namespace Neptune.Transport.Core.Events;

/// <summary>
/// Event arguments for transport status changed events
/// </summary>
public class TransportStatusChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the transport that changed status
    /// </summary>
    public ITransport Transport { get; }

    /// <summary>
    /// Gets whether the transport is active
    /// </summary>
    public bool IsActive { get; }

    public TransportStatusChangedEventArgs(ITransport transport, bool isActive)
    {
        Transport = transport;
        IsActive = isActive;
    }
}
