using Neptune.Packets.Messages;
using Neptune.Transport.Core.Interfaces;
using Neptune.Transport.Core.Models;

namespace Neptune.Transport.Core.Events;

/// <summary>
/// Event arguments for message received events
/// </summary>
public class MessageReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the received message
    /// </summary>
    public NeptuneMessage Message { get; }

    /// <summary>
    /// Gets information about the source peer
    /// </summary>
    public PeerInfo SourcePeer { get; }

    /// <summary>
    /// Gets the transport that received the message
    /// </summary>
    public ITransport Transport { get; }

    public MessageReceivedEventArgs(NeptuneMessage message, PeerInfo sourcePeer, ITransport transport)
    {
        Message = message;
        SourcePeer = sourcePeer;
        Transport = transport;
    }
}
