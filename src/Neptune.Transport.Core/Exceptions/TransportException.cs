namespace Neptune.Transport.Core.Exceptions;

/// <summary>
/// Base exception for transport-related issues
/// </summary>
public class TransportException : Exception
{
    public string TransportId { get; }

    public TransportException(string message, string transportId = null)
        : base(message)
    {
        TransportId = transportId;
    }

    public TransportException(string message, Exception innerException, string transportId = null)
        : base(message, innerException)
    {
        TransportId = transportId;
    }
}
