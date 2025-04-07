using Neptune.Transport.Core.Interfaces;
using Neptune.Transport.Core.Models;

namespace Neptune.Transport.Core.Events;

/// <summary>
/// Event arguments for peer discovered events
/// </summary>
public class PeerDiscoveredEventArgs : EventArgs
{
    /// <summary>
    /// Gets information about the discovered peer
    /// </summary>
    public PeerInfo PeerInfo { get; }

    /// <summary>
    /// Gets the transport that discovered the peer
    /// </summary>
    public ITransport Transport { get; }

    public PeerDiscoveredEventArgs(PeerInfo peerInfo, ITransport transport)
    {
        PeerInfo = peerInfo;
        Transport = transport;
    }
}
