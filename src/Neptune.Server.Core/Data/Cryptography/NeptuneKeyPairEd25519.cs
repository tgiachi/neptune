using NSec.Cryptography;

namespace Neptune.Server.Core.Data.Cryptography;

public class NeptuneKeyPairEd25519
{
    public Key PrivateKey { get; private set; }
    public PublicKey PublicKey { get; private set; }

    private static readonly SignatureAlgorithm Algorithm = SignatureAlgorithm.Ed25519;


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

    public static NeptuneKeyPairEd25519 FromPrivateKey(byte[] privateKeyBytes)
    {
        var privateKey = Key.Import(Algorithm, privateKeyBytes, KeyBlobFormat.RawPrivateKey);
        return new NeptuneKeyPairEd25519
        {
            PrivateKey = privateKey,
            PublicKey = privateKey.PublicKey
        };
    }

    public static NeptuneKeyPairEd25519 FromPrivateKey(string base64)
    {
        return FromPrivateKey(Convert.FromBase64String(base64));
    }


    public static PublicKey ImportPublicKey(byte[] publicKeyBytes)
    {
        return PublicKey.Import(Algorithm, publicKeyBytes, KeyBlobFormat.RawPublicKey);
    }

    public static PublicKey ImportPublicKey(string base64)
    {
        return ImportPublicKey(Convert.FromBase64String(base64));
    }

    // Export
    public byte[] ExportPrivateKey() =>
        PrivateKey.Export(KeyBlobFormat.RawPrivateKey);

    public byte[] ExportPublicKey() =>
        PublicKey.Export(KeyBlobFormat.RawPublicKey);

    public string ExportPrivateKeyBase64() =>
        Convert.ToBase64String(ExportPrivateKey());

    public string ExportPublicKeyBase64() =>
        Convert.ToBase64String(ExportPublicKey());


    public byte[] Sign(byte[] data) =>
        Algorithm.Sign(PrivateKey, data);

    public bool Verify(byte[] data, byte[] signature) =>
        Algorithm.Verify(PublicKey, data, signature);
}
