using System.Text;
using Neptune.Server.Core.Data.Cryptography;
using NSec.Cryptography;

namespace Neptune.Tests;

[TestFixture]
public class NeptuneKeyPairEd25519Tests
{
    [Test]
    public void Generate_ShouldCreateValidKeys()
    {
        var kp = new NeptuneKeyPairEd25519();
        kp.Generate();

        var pub = kp.ExportPublicKey();
        var priv = kp.ExportPrivateKey();

        Assert.That(pub, Is.Not.Null);
        Assert.That(priv, Is.Not.Null);
        Assert.That(pub.Length, Is.EqualTo(32));  // Ed25519 public key
        Assert.That(priv.Length, Is.EqualTo(32)); // Ed25519 private key
    }

    [Test]
    public void Sign_And_Verify_ShouldBeValid()
    {
        var kp = new NeptuneKeyPairEd25519();
        kp.Generate();

        var data = Encoding.UTF8.GetBytes("neptune rocks!");
        var signature = kp.Sign(data);

        Assert.That(signature, Is.Not.Null);
        Assert.That(signature.Length, Is.EqualTo(64));
        Assert.That(kp.Verify(data, signature), Is.True);
    }

    [Test]
    public void ExportImport_PrivateKey_ShouldPreserveFunctionality()
    {
        var original = new NeptuneKeyPairEd25519();
        original.Generate();

        var exportedBase64 = original.ExportPrivateKeyBase64();
        var restored = NeptuneKeyPairEd25519.FromPrivateKey(exportedBase64);

        var data = Encoding.UTF8.GetBytes("cross-import signature");
        var signature = restored.Sign(data);

        Assert.That(restored.Verify(data, signature), Is.True);
        Assert.That(original.Verify(data, signature), Is.True);
    }

    [Test]
    public void ExportImport_PublicKey_ShouldVerifySignature()
    {
        var kp = new NeptuneKeyPairEd25519();
        kp.Generate();

        var data = Encoding.UTF8.GetBytes("public key verify test");
        var signature = kp.Sign(data);

        var importedPub = NeptuneKeyPairEd25519.ImportPublicKey(kp.ExportPublicKeyBase64());
        var verified = SignatureAlgorithm.Ed25519.Verify(importedPub, data, signature);

        Assert.That(verified, Is.True);
    }

    [Test]
    public void Verify_ShouldFail_WithWrongKey()
    {
        var alice = new NeptuneKeyPairEd25519();
        var bob = new NeptuneKeyPairEd25519();
        alice.Generate();
        bob.Generate();

        var message = Encoding.UTF8.GetBytes("not yours");
        var signature = alice.Sign(message);

        Assert.That(bob.Verify(message, signature), Is.False);
    }
}
