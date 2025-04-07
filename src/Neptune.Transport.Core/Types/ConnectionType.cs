namespace Neptune.Transport.Core.Types;

/// <summary>
/// Connection type with a peer
/// </summary>
public enum ConnectionType
{
    /// <summary>
    /// Not connected
    /// </summary>
    None,

    /// <summary>
    /// Connected directly
    /// </summary>
    Direct,

    /// <summary>
    /// Connected through a relay
    /// </summary>
    Relayed,

    /// <summary>
    /// Connected intermittently
    /// </summary>
    Intermittent
}
