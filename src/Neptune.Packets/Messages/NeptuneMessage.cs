using System;
using System.Collections.Generic;
using Neptune.Packets.Types;

namespace Neptune.Packets.Messages;

/// <summary>
/// Represents a Neptune Protocol message.
/// </summary>
public class NeptuneMessage
{
    /// <summary>
    /// Protocol version
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Message header containing metadata
    /// </summary>
    public MessageHeader Header { get; set; } = new MessageHeader();

    /// <summary>
    /// Routing information for message delivery
    /// </summary>
    public MessageRouting Routing { get; set; } = new MessageRouting();

    /// <summary>
    /// Cryptographic information
    /// </summary>
    public CryptoInfo Crypto { get; set; } = new CryptoInfo();

    /// <summary>
    /// Message payload
    /// </summary>
    public EncryptedPayload Payload { get; set; } = new EncryptedPayload();

    /// <summary>
    /// Creates a new Neptune message with default values
    /// </summary>
    public NeptuneMessage()
    {
        // Default values are set through property initializers
    }

    /// <summary>
    /// Creates a channel message
    /// </summary>
    /// <param name="senderId">Sender ID in the format localID@server.domain</param>
    /// <param name="channel">Target channel name</param>
    /// <param name="text">Message text content</param>
    /// <returns>A configured message (not yet encrypted)</returns>
    public static NeptuneMessage CreateChannelMessage(string senderId, string channel, string text)
    {
        if (string.IsNullOrEmpty(senderId))
        {
            throw new ArgumentNullException(nameof(senderId));
        }

        if (string.IsNullOrEmpty(channel))
        {
            throw new ArgumentNullException(nameof(channel));
        }

        return new NeptuneMessage
        {
            Header = new MessageHeader
            {
                SenderId = senderId,
                Type = MessageType.MESSAGE,
                EncryptionType = EncryptionType.CHANNEL
            },
            Routing = new MessageRouting
            {
                Channel = channel
            },
            Payload = new EncryptedPayload
            {
                Data = text, // Note: This should be encrypted before sending
                Format = PayloadFormat.TEXT,
                ContentType = "text/plain"
            }
        };
    }

    /// <summary>
    /// Creates a private message
    /// </summary>
    /// <param name="senderId">Sender ID in the format localID@server.domain</param>
    /// <param name="recipientId">Recipient ID in the format localID@server.domain</param>
    /// <param name="text">Message text content</param>
    /// <returns>A configured message (not yet encrypted)</returns>
    public static NeptuneMessage CreatePrivateMessage(string senderId, string recipientId, string text)
    {
        if (string.IsNullOrEmpty(senderId))
        {
            throw new ArgumentNullException(nameof(senderId));
        }

        if (string.IsNullOrEmpty(recipientId))
        {
            throw new ArgumentNullException(nameof(recipientId));
        }

        return new NeptuneMessage
        {
            Header = new MessageHeader
            {
                SenderId = senderId,
                Type = MessageType.PRIVMSG,
                EncryptionType = EncryptionType.E2E
            },
            Routing = new MessageRouting
            {
                Recipient = recipientId
            },
            Payload = new EncryptedPayload
            {
                Data = text, // Note: This should be encrypted before sending
                Format = PayloadFormat.TEXT,
                ContentType = "text/plain"
            }
        };
    }

    /// <summary>
    /// Creates a message to join a channel
    /// </summary>
    /// <param name="senderId">Sender ID in the format localID@server.domain</param>
    /// <param name="channel">Channel name to join</param>
    /// <returns>A configured JOIN message</returns>
    public static NeptuneMessage CreateJoinMessage(string senderId, string channel)
    {
        if (string.IsNullOrEmpty(senderId))
        {
            throw new ArgumentNullException(nameof(senderId));
        }

        if (string.IsNullOrEmpty(channel))
        {
            throw new ArgumentNullException(nameof(channel));
        }

        return new NeptuneMessage
        {
            Header = new MessageHeader
            {
                SenderId = senderId,
                Type = MessageType.JOIN,
                EncryptionType = EncryptionType.NONE
            },
            Routing = new MessageRouting
            {
                Channel = channel
            },
            Payload = new EncryptedPayload
            {
                Data = string.Empty,
                Format = PayloadFormat.TEXT,
                ContentType = "text/plain"
            }
        };
    }

    /// <summary>
    /// Creates a message to leave a channel
    /// </summary>
    /// <param name="senderId">Sender ID in the format localID@server.domain</param>
    /// <param name="channel">Channel name to leave</param>
    /// <returns>A configured LEAVE message</returns>
    public static NeptuneMessage CreateLeaveMessage(string senderId, string channel)
    {
        if (string.IsNullOrEmpty(senderId))
            throw new ArgumentNullException(nameof(senderId));

        if (string.IsNullOrEmpty(channel))
            throw new ArgumentNullException(nameof(channel));

        return new NeptuneMessage
        {
            Header = new MessageHeader
            {
                SenderId = senderId,
                Type = MessageType.LEAVE,
                EncryptionType = EncryptionType.NONE
            },
            Routing = new MessageRouting
            {
                Channel = channel
            },
            Payload = new EncryptedPayload
            {
                Data = string.Empty,
                Format = PayloadFormat.TEXT,
                ContentType = "text/plain"
            }
        };
    }

    /// <summary>
    /// Creates a ping message to check connection
    /// </summary>
    /// <param name="senderId">Sender ID in the format localID@server.domain</param>
    /// <returns>A configured PING message</returns>
    public static NeptuneMessage CreatePingMessage(string senderId)
    {
        if (string.IsNullOrEmpty(senderId))
        {
            throw new ArgumentNullException(nameof(senderId));
        }

        return new NeptuneMessage
        {
            Header = new MessageHeader
            {
                SenderId = senderId,
                Type = MessageType.PING,
                EncryptionType = EncryptionType.NONE
            },
            Payload = new EncryptedPayload
            {
                Data = DateTimeOffset.UtcNow.ToString("o"),
                Format = PayloadFormat.TEXT,
                ContentType = "text/plain"
            }
        };
    }

    /// <summary>
    /// Creates a pong message in response to a ping
    /// </summary>
    /// <param name="senderId">Sender ID in the format localID@server.domain</param>
    /// <param name="pingMessage">The original ping message</param>
    /// <returns>A configured PONG message</returns>
    public static NeptuneMessage CreatePongMessage(string senderId, NeptuneMessage pingMessage)
    {
        if (string.IsNullOrEmpty(senderId))
        {
            throw new ArgumentNullException(nameof(senderId));
        }

        if (pingMessage == null)
        {
            throw new ArgumentNullException(nameof(pingMessage));
        }

        if (pingMessage.Header.Type != MessageType.PING)
        {
            throw new ArgumentException("The provided message is not a PING message", nameof(pingMessage));
        }

        return new NeptuneMessage
        {
            Header = new MessageHeader
            {
                SenderId = senderId,
                Type = MessageType.PONG,
                EncryptionType = EncryptionType.NONE
            },
            Routing = new MessageRouting
            {
                Recipient = pingMessage.Header.SenderId
            },
            Payload = new EncryptedPayload
            {
                Data = pingMessage.Payload.Data,
                Format = PayloadFormat.TEXT,
                ContentType = "text/plain"
            }
        };
    }

    /// <summary>
    /// Creates an error message
    /// </summary>
    /// <param name="senderId">Sender ID in the format localID@server.domain</param>
    /// <param name="recipientId">Recipient ID in the format localID@server.domain</param>
    /// <param name="errorMessage">Error message text</param>
    /// <param name="errorCode">Optional error code</param>
    /// <returns>A configured ERROR message</returns>
    public static NeptuneMessage CreateErrorMessage(
        string senderId, string recipientId, string errorMessage, string errorCode = null
    )
    {
        if (string.IsNullOrEmpty(senderId))
            throw new ArgumentNullException(nameof(senderId));

        if (string.IsNullOrEmpty(recipientId))
            throw new ArgumentNullException(nameof(recipientId));

        if (string.IsNullOrEmpty(errorMessage))
            throw new ArgumentNullException(nameof(errorMessage));

        var payload = new Dictionary<string, string>
        {
            ["message"] = errorMessage
        };

        if (!string.IsNullOrEmpty(errorCode))
        {
            payload["code"] = errorCode;
        }

        return new NeptuneMessage
        {
            Header = new MessageHeader
            {
                SenderId = senderId,
                Type = MessageType.ERROR,
                EncryptionType = EncryptionType.NONE
            },
            Routing = new MessageRouting
            {
                Recipient = recipientId
            },
            Payload = new EncryptedPayload
            {
                Data = System.Text.Json.JsonSerializer.Serialize(payload),
                Format = PayloadFormat.JSON,
                ContentType = "application/json"
            }
        };
    }
}
