using System;

namespace Neptune.Packets.Messages;

/// <summary>
/// A key pair for cryptographic operations
/// </summary>
public class KeyPair
{
    /// <summary>
    /// Public key (base64 encoded)
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Private key (base64 encoded)
    /// </summary>
    public string PrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// Identifier derived from the public key (format: algorithm:key-hash)
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Friendly name for the key pair
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// When the key pair was created
    /// </summary>
    public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Optional expiration date for the key pair
    /// </summary>
    public DateTimeOffset? Expires { get; set; }
}
