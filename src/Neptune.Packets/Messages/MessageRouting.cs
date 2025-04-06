using System.Collections.Generic;

namespace Neptune.Packets.Messages;

/// <summary>
/// Routing information for a Neptune message
/// </summary>
public class MessageRouting
{
    /// <summary>
    /// Target channel name (for channel messages)
    /// </summary>
    public string? Channel { get; set; }

    /// <summary>
    /// Target recipient ID (for private messages)
    /// </summary>
    public string? Recipient { get; set; }

    /// <summary>
    /// Transport-specific metadata
    /// </summary>
    public Dictionary<string, object> TransportMetadata { get; set; } = new Dictionary<string, object>();
}
