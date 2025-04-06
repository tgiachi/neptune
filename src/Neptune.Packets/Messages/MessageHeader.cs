using System;
using Neptune.Packets.Types;

namespace Neptune.Packets.Messages;

/// <summary>
/// Header information for a Neptune message
/// </summary>
public class MessageHeader
{
    /// <summary>
    /// Unique identifier for the message
    /// </summary>
    public string MessageId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Sender's identifier in the format "localID@server.domain"
    /// </summary>
    public string SenderId { get; set; } = string.Empty;

    /// <summary>
    /// Unix timestamp (seconds since epoch)
    /// </summary>
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <summary>
    /// Type of the message
    /// </summary>
    public MessageType Type { get; set; } = MessageType.MESSAGE;

    /// <summary>
    /// Type of encryption used
    /// </summary>
    public EncryptionType EncryptionType { get; set; } = EncryptionType.NONE;
}

