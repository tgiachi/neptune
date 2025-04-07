namespace Neptune.Transport.Core.Exceptions;

/// <summary>
/// Exception thrown when a message cannot be sent
/// </summary>
public class MessageSendException : TransportException
{
    public string MessageId { get; }

    public MessageSendException(string message, string messageId = null, string transportId = null)
        : base(message, transportId)
    {
        MessageId = messageId;
    }

    public MessageSendException(string message, Exception innerException, string messageId = null, string transportId = null)
        : base(message, innerException, transportId)
    {
        MessageId = messageId;
    }
}
