using System.Net;

namespace Neptune.Transport.Core.Models;

/// <summary>
/// Contains endpoint information for a peer
/// </summary>
public class EndpointInfo
{
    /// <summary>
    /// Gets or sets the IP endpoint
    /// </summary>
    public IPEndPoint IpEndPoint { get; set; }

    /// <summary>
    /// Gets or sets the Bluetooth address
    /// </summary>
    public string BluetoothAddress { get; set; }

    /// <summary>
    /// Gets or sets the LoRa address
    /// </summary>
    public string LoRaAddress { get; set; }

    /// <summary>
    /// Gets or sets a custom address
    /// </summary>
    public string CustomAddress { get; set; }
}
