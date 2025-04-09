using System.Security.Cryptography;
using System.Text;

namespace Neptune.Server.Core.Data.Cryptography;

  /// <summary>
    /// Represents a cryptographic object that encapsulates RSA keys and operations
    /// </summary>
    public class NeptuneCryptObject
    {
        private RSA _rsa;
        private readonly int _keySize;

        /// <summary>
        /// Gets a value indicating whether this object has a public key loaded
        /// </summary>
        public bool HasPublicKey => _rsa != null;

        /// <summary>
        /// Gets a value indicating whether this object has a private key loaded
        /// </summary>
        public bool HasPrivateKey
        {
            get
            {
                try
                {
                    if (_rsa == null) return false;
                    // Try exporting the private key - will throw if not present
                    _rsa.ExportRSAPrivateKey();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Creates a new NeptuneCryptObject with the specified key size
        /// </summary>
        /// <param name="keySize">Size of the RSA key in bits. Default is 2048.</param>
        public NeptuneCryptObject(int keySize = 2048)
        {
            _keySize = keySize;
        }

        /// <summary>
        /// Generates a new key pair (public and private keys)
        /// </summary>
        /// <returns>This instance for method chaining</returns>
        public NeptuneCryptObject GenerateKeyPair()
        {
            _rsa = RSA.Create(_keySize);
            return this;
        }

        /// <summary>
        /// Exports the public key as a Base64 string
        /// </summary>
        /// <returns>The public key in Base64 format</returns>
        public string ExportPublicKeyBase64()
        {
            if (_rsa == null)
                throw new InvalidOperationException("No key is loaded. Generate or import a key first.");

            byte[] publicKey = _rsa.ExportRSAPublicKey();
            return Convert.ToBase64String(publicKey);
        }

        /// <summary>
        /// Exports the private key as a Base64 string
        /// </summary>
        /// <returns>The private key in Base64 format</returns>
        public string ExportPrivateKeyBase64()
        {
            if (_rsa == null)
                throw new InvalidOperationException("No key is loaded. Generate or import a key first.");

            if (!HasPrivateKey)
                throw new InvalidOperationException("No private key is available in this object.");

            byte[] privateKey = _rsa.ExportRSAPrivateKey();
            return Convert.ToBase64String(privateKey);
        }

        /// <summary>
        /// Imports a public key from a Base64 string
        /// </summary>
        /// <param name="base64PublicKey">The public key in Base64 format</param>
        /// <returns>This instance for method chaining</returns>
        public NeptuneCryptObject ImportPublicKeyBase64(string base64PublicKey)
        {
            if (string.IsNullOrEmpty(base64PublicKey))
                throw new ArgumentNullException(nameof(base64PublicKey));

            byte[] publicKeyBytes = Convert.FromBase64String(base64PublicKey);

            _rsa = RSA.Create();
            _rsa.ImportRSAPublicKey(publicKeyBytes, out _);

            return this;
        }

        /// <summary>
        /// Imports a private key from a Base64 string
        /// </summary>
        /// <param name="base64PrivateKey">The private key in Base64 format</param>
        /// <returns>This instance for method chaining</returns>
        public NeptuneCryptObject ImportPrivateKeyBase64(string base64PrivateKey)
        {
            if (string.IsNullOrEmpty(base64PrivateKey))
                throw new ArgumentNullException(nameof(base64PrivateKey));

            byte[] privateKeyBytes = Convert.FromBase64String(base64PrivateKey);

            _rsa = RSA.Create();
            _rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            return this;
        }

        /// <summary>
        /// Signs a message using the private key
        /// </summary>
        /// <param name="message">The message to sign</param>
        /// <returns>The signature in Base64 format</returns>
        public string SignMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            if (_rsa == null || !HasPrivateKey)
                throw new InvalidOperationException("A private key is required for signing.");

            byte[] dataToSign = Encoding.UTF8.GetBytes(message);
            byte[] signature = _rsa.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signature);
        }

        /// <summary>
        /// Verifies a signature against a message using the public key
        /// </summary>
        /// <param name="message">The original message</param>
        /// <param name="base64Signature">The signature in Base64 format</param>
        /// <returns>True if the signature is valid; otherwise, false</returns>
        public bool VerifySignature(string message, string base64Signature)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrEmpty(base64Signature))
                throw new ArgumentNullException(nameof(base64Signature));

            if (_rsa == null)
                throw new InvalidOperationException("A public key is required for verification.");

            byte[] dataToVerify = Encoding.UTF8.GetBytes(message);
            byte[] signature = Convert.FromBase64String(base64Signature);

            return _rsa.VerifyData(dataToVerify, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        /// <summary>
        /// Encrypts a message using the public key
        /// </summary>
        /// <param name="plainText">The message to encrypt</param>
        /// <returns>The encrypted message in Base64 format</returns>
        public string EncryptMessage(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            if (_rsa == null)
                throw new InvalidOperationException("A public key is required for encryption.");

            // For longer messages, we use a hybrid approach with AES
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Encrypt the AES key with RSA
                byte[] encryptedAesKey = _rsa.Encrypt(aes.Key, RSAEncryptionPadding.OaepSHA256);
                byte[] encryptedAesIV = _rsa.Encrypt(aes.IV, RSAEncryptionPadding.OaepSHA256);

                // Encrypt the actual data with AES
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    byte[] encryptedData = msEncrypt.ToArray();

                    // Combine everything: [Key length (4 bytes)][Encrypted AES Key][IV length (4 bytes)][Encrypted AES IV][Encrypted Data]
                    using (MemoryStream combinedStream = new MemoryStream())
                    {
                        using (BinaryWriter writer = new BinaryWriter(combinedStream))
                        {
                            writer.Write(encryptedAesKey.Length);
                            writer.Write(encryptedAesKey);
                            writer.Write(encryptedAesIV.Length);
                            writer.Write(encryptedAesIV);
                            writer.Write(encryptedData);
                        }

                        return Convert.ToBase64String(combinedStream.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts a message using the private key
        /// </summary>
        /// <param name="base64CipherText">The encrypted message in Base64 format</param>
        /// <returns>The decrypted message</returns>
        public string DecryptMessage(string base64CipherText)
        {
            if (string.IsNullOrEmpty(base64CipherText))
                throw new ArgumentNullException(nameof(base64CipherText));

            if (_rsa == null || !HasPrivateKey)
                throw new InvalidOperationException("A private key is required for decryption.");

            byte[] cipherBytes = Convert.FromBase64String(base64CipherText);

            using (MemoryStream combinedStream = new MemoryStream(cipherBytes))
            {
                using (BinaryReader reader = new BinaryReader(combinedStream))
                {
                    // Read the encrypted AES key
                    int keyLength = reader.ReadInt32();
                    byte[] encryptedAesKey = reader.ReadBytes(keyLength);

                    // Read the encrypted AES IV
                    int ivLength = reader.ReadInt32();
                    byte[] encryptedAesIV = reader.ReadBytes(ivLength);

                    // Read the encrypted data
                    byte[] encryptedData = reader.ReadBytes((int)(combinedStream.Length - combinedStream.Position));

                    // Decrypt the AES key and IV with RSA
                    byte[] aesKey = _rsa.Decrypt(encryptedAesKey, RSAEncryptionPadding.OaepSHA256);
                    byte[] aesIV = _rsa.Decrypt(encryptedAesIV, RSAEncryptionPadding.OaepSHA256);

                    // Decrypt the data with AES
                    using (Aes aes = Aes.Create())
                    {
                        aes.KeySize = 256;
                        aes.Key = aesKey;
                        aes.IV = aesIV;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        ICryptoTransform decryptor = aes.CreateDecryptor();

                        using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                {
                                    return srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
