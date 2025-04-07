namespace Neptune.Transport.Core.Exceptions;

/// <summary>
/// Exception thrown when a transport fails to initialize
/// </summary>
public class TransportInitializationException : TransportException
{
    public TransportInitializationException(string message, string transportId = null)
        : base(message, transportId)
    {
    }

    public TransportInitializationException(string message, Exception innerException, string transportId = null)
        : base(message, innerException, transportId)
    {
    }
}
