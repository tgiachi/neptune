namespace Neptune.Packets.Types;

/// <summary>
/// The type of encryption used for a message.
/// </summary>
public enum EncryptionType
{
    /// <summary>
    /// End-to-end encryption with recipient's public key
    /// </summary>
    E2E,

    /// <summary>
    /// Channel encryption with shared key
    /// </summary>
    CHANNEL,

    /// <summary>
    /// No encryption (not recommended for production)
    /// </summary>
    NONE
}
