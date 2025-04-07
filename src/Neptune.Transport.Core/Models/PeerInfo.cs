using Neptune.Transport.Core.Types;

namespace Neptune.Transport.Core.Models;

/// <summary>
/// Contains information about a peer node
/// </summary>
public class PeerInfo
{
    /// <summary>
    /// Gets the unique node ID
    /// </summary>
    public string NodeId { get; set; }

    /// <summary>
    /// Gets the endpoint information
    /// </summary>
    public EndpointInfo Endpoint { get; set; }

    /// <summary>
    /// Gets the last seen timestamp
    /// </summary>
    public DateTime LastSeen { get; set; }

    /// <summary>
    /// Gets the connection type with this peer
    /// </summary>
    public ConnectionType ConnectionType { get; set; }

    /// <summary>
    /// Gets the transport IDs this peer is available on
    /// </summary>
    public List<string> AvailableTransports { get; set; } = new List<string>();

    /// <summary>
    /// Gets the round-trip time in milliseconds
    /// </summary>
    public long RoundTripTimeMs { get; set; }

    /// <summary>
    /// Gets additional peer capabilities
    /// </summary>
    public Dictionary<string, string> Capabilities { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets the signal strength (if applicable)
    /// </summary>
    public int? SignalStrength { get; set; }

    /// <summary>
    /// Gets or sets the battery level (if available)
    /// </summary>
    public int? BatteryLevel { get; set; }
}
