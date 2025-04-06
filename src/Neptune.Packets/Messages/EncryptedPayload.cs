using Neptune.Packets.Types;

namespace Neptune.Packets.Messages;

/// <summary>
/// Encrypted payload for a Neptune message
/// </summary>
public class EncryptedPayload
{
    /// <summary>
    /// Message content (encrypted and base64 encoded if encryption is used)
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// Format of the decrypted content
    /// </summary>
    public PayloadFormat Format { get; set; } = PayloadFormat.TEXT;

    /// <summary>
    /// MIME content type
    /// </summary>
    public string ContentType { get; set; } = "text/plain";
}
