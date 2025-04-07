namespace Neptune.Transport.Core.Exceptions;

/// <summary>
/// Exception thrown when peer discovery fails
/// </summary>
public class PeerDiscoveryException : TransportException
{
    public PeerDiscoveryException(string message, string transportId = null)
        : base(message, transportId)
    {
    }

    public PeerDiscoveryException(string message, Exception innerException, string transportId = null)
        : base(message, innerException, transportId)
    {
    }
}
