namespace Neptune.Packets.Messages;

/// <summary>
/// Cryptographic information for a Neptune message
/// </summary>
public class CryptoInfo
{
    /// <summary>
    /// Sender's public key (base64 encoded)
    /// </summary>
    public string? PublicKey { get; set; }

    /// <summary>
    /// Channel key identifier (for channel encryption)
    /// </summary>
    public string? ChannelKeyId { get; set; }

    /// <summary>
    /// Initialization vector for encryption (base64 encoded)
    /// </summary>
    public string? IV { get; set; }

    /// <summary>
    /// Digital signature of the message (base64 encoded)
    /// </summary>
    public string? Signature { get; set; }
}
