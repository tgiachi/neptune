namespace Neptune.Packets.Types;

/// <summary>
/// The type of message in the Neptune protocol.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Regular message sent to a channel
    /// </summary>
    MESSAGE,

    /// <summary>
    /// Private message sent to a specific user
    /// </summary>
    PRIVMSG,

    /// <summary>
    /// Request to join a channel
    /// </summary>
    JOIN,

    /// <summary>
    /// Request to leave a channel
    /// </summary>
    LEAVE,

    /// <summary>
    /// Connection check
    /// </summary>
    PING,

    /// <summary>
    /// Response to a ping
    /// </summary>
    PONG,

    /// <summary>
    /// Channel or server information
    /// </summary>
    INFO,

    /// <summary>
    /// Error notification
    /// </summary>
    ERROR
}
