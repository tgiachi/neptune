using NSec.Cryptography;
using System;
using System.Text;
using System.Security.Cryptography;

namespace Neptune.Server.Core.Data.Cryptography;

public class NeptuneEncryptorX25519
{
    public Key PrivateKey { get; private set; }
    public PublicKey PublicKey { get; private set; }

    private static readonly KeyAgreementAlgorithm Algorithm = KeyAgreementAlgorithm.X25519;
    private static readonly KeyDerivationAlgorithm KdfAlgorithm = KeyDerivationAlgorithm.HkdfSha256;
    private static readonly AeadAlgorithm EncryptionAlgorithm = AeadAlgorithm.Aes256Gcm;

    public NeptuneEncryptorX25519()
    {
    }

    public void Generate()
    {
        PrivateKey = Key.Create(
            Algorithm,
            new KeyCreationParameters
            {
                ExportPolicy = KeyExportPolicies.AllowPlaintextExport
            }
        );
        PublicKey = PrivateKey.PublicKey;
    }

    public static NeptuneEncryptorX25519 FromPrivateKey(byte[] privateKeyBytes)
    {
        var privateKey = Key.Import(Algorithm, privateKeyBytes, KeyBlobFormat.RawPrivateKey);
        return new NeptuneEncryptorX25519
        {
            PrivateKey = privateKey,
            PublicKey = privateKey.PublicKey
        };
    }

    public static NeptuneEncryptorX25519 FromPrivateKey(string base64)
    {
        return FromPrivateKey(Convert.FromBase64String(base64));
    }

    public static PublicKey ImportPublicKey(byte[] publicKeyBytes)
    {
        return PublicKey.Import(Algorithm, publicKeyBytes, KeyBlobFormat.RawPublicKey);
    }

    public static PublicKey ImportPublicKey(string base64)
    {
        if (string.IsNullOrEmpty(base64))
        {
            throw new ArgumentException("Public key cannot be empty", nameof(base64));
        }

        try
        {
            return ImportPublicKey(Convert.FromBase64String(base64));
        }
        catch (FormatException)
        {
            throw new ArgumentException("Public key is not in valid Base64 format", nameof(base64));
        }
    }

    public static bool IsValidPublicKey(string base64)
    {
        if (string.IsNullOrEmpty(base64))
        {
            return false;
        }

        try
        {
            var keyBytes = Convert.FromBase64String(base64);
            PublicKey.Import(Algorithm, keyBytes, KeyBlobFormat.RawPublicKey);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Export methods
    public byte[] ExportPrivateKey() =>
        PrivateKey.Export(KeyBlobFormat.RawPrivateKey);

    public byte[] ExportPublicKey() =>
        PublicKey.Export(KeyBlobFormat.RawPublicKey);

    public string ExportPrivateKeyBase64() =>
        Convert.ToBase64String(ExportPrivateKey());

    public string ExportPublicKeyBase64() =>
        Convert.ToBase64String(ExportPublicKey());


    // Encryption
    public byte[] Encrypt(byte[] plaintext, PublicKey recipientPublicKey)
    {
        // Generate a random nonce
        var nonce = new byte[EncryptionAlgorithm.NonceSize];
        RandomNumberGenerator.Fill(nonce);

        // Perform key agreement using X25519 algorithm
        var sharedSecret = Algorithm.Agree(PrivateKey, recipientPublicKey);

        if (sharedSecret == null)
        {
            throw new InvalidOperationException("Failed to generate shared secret");
        }

        // Derive encryption key from shared secret using HKDF
        var encryptionKey = KdfAlgorithm.DeriveKey(
            sharedSecret,
            nonce,
            Array.Empty<byte>(),
            EncryptionAlgorithm,
            new KeyCreationParameters()
        );

        // Encrypt the data
        var ciphertext = EncryptionAlgorithm.Encrypt(
            encryptionKey,
            nonce,
            Array.Empty<byte>(),
            plaintext
        );

        // Combine nonce and ciphertext into a single array
        var result = new byte[nonce.Length + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
        Buffer.BlockCopy(ciphertext, 0, result, nonce.Length, ciphertext.Length);

        return result;
    }

    public string EncryptToBase64(string plaintext, PublicKey recipientPublicKey)
    {
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var encryptedBytes = Encrypt(plaintextBytes, recipientPublicKey);
        return Convert.ToBase64String(encryptedBytes);
    }

    // Decryption
    public byte[] Decrypt(byte[] encryptedData, PublicKey senderPublicKey)
    {
        if (encryptedData.Length < EncryptionAlgorithm.NonceSize)
        {
            throw new ArgumentException("Encrypted data is too short", nameof(encryptedData));
        }

        // Extract nonce and ciphertext
        var nonce = new byte[EncryptionAlgorithm.NonceSize];
        Buffer.BlockCopy(encryptedData, 0, nonce, 0, nonce.Length);

        var ciphertext = new byte[encryptedData.Length - nonce.Length];
        Buffer.BlockCopy(encryptedData, nonce.Length, ciphertext, 0, ciphertext.Length);

        // Perform key agreement using X25519 algorithm
        var sharedSecret = Algorithm.Agree(PrivateKey, senderPublicKey);

        if (sharedSecret == null)
        {
            throw new InvalidOperationException("Failed to generate shared secret");
        }

        // Derive encryption key from shared secret using HKDF
        var encryptionKey = KdfAlgorithm.DeriveKey(
            sharedSecret,
            nonce,
            Array.Empty<byte>(),
            EncryptionAlgorithm,
            new KeyCreationParameters()
        );

        // Decrypt the data
        return EncryptionAlgorithm.Decrypt(
            encryptionKey,
            nonce,
            Array.Empty<byte>(),
            ciphertext
        );
    }

    public string DecryptFromBase64(string base64, PublicKey senderPublicKey)
    {
        var encryptedBytes = Convert.FromBase64String(base64);
        var decryptedBytes = Decrypt(encryptedBytes, senderPublicKey);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
