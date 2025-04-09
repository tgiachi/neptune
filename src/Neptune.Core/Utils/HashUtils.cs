using System.Security.Cryptography;
using System.Text;

namespace Neptune.Core.Utils;

public class HashUtils
{
    public static string ComputeSha256Hash(string rawData)
    {
        using SHA256 sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        var builder = new StringBuilder();
        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }

    public static (string Hash, string Salt) HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(32); // 256-bit hash
            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }
    }

    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] salt = Convert.FromBase64String(storedSalt);
        byte[] expectedHash = Convert.FromBase64String(storedHash);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);
        return CryptographicOperations.FixedTimeEquals(hash, expectedHash);
    }

    public static string GenerateRandomRefreshToken(int size = 32)
    {
        byte[] randomBytes = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return Convert.ToBase64String(randomBytes);
    }

    public static string GenerateBase64Key(int byteLength = 32)
    {
        var key = new byte[byteLength];
        RandomNumberGenerator.Fill(key);
        return Convert.ToBase64String(key);
    }

    public static byte[] Encrypt(string plaintext, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
        var cipherBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

        // concatena IV + CIPHERTEXT
        return aes.IV.Concat(cipherBytes).ToArray();
    }

    public static string Decrypt(byte[] ivAndCiphertext, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;

        aes.IV = ivAndCiphertext[..16];
        var cipher = ivAndCiphertext[16..];

        using var decryptor = aes.CreateDecryptor();
        var plaintextBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        return System.Text.Encoding.UTF8.GetString(plaintextBytes);
    }
}
