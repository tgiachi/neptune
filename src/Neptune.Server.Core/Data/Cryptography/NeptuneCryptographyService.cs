namespace Neptune.Server.Core.Data.Cryptography;

 /// <summary>
    /// Service for managing cryptographic operations and creating NeptuneCryptObjects
    /// </summary>
    public class NeptuneCryptographyService
    {
        private readonly int _defaultKeySize;

        /// <summary>
        /// Initializes a new instance of the NeptuneCryptographyService
        /// </summary>
        /// <param name="defaultKeySize">Default key size in bits to use when creating new keys</param>
        public NeptuneCryptographyService(int defaultKeySize = 2048)
        {
            _defaultKeySize = defaultKeySize;
        }

        /// <summary>
        /// Creates a new NeptuneCryptObject with a fresh key pair
        /// </summary>
        /// <param name="keySize">Key size in bits, or null to use the default</param>
        /// <returns>A new NeptuneCryptObject with generated keys</returns>
        public NeptuneCryptObject CreateWithNewKeyPair(int? keySize = null)
        {
            var cryptObject = new NeptuneCryptObject(keySize ?? _defaultKeySize);
            cryptObject.GenerateKeyPair();
            return cryptObject;
        }

        /// <summary>
        /// Creates a NeptuneCryptObject with a public key imported from Base64
        /// </summary>
        /// <param name="base64PublicKey">The public key in Base64 format</param>
        /// <param name="keySize">Key size in bits, or null to use the default</param>
        /// <returns>A new NeptuneCryptObject with the imported public key</returns>
        public NeptuneCryptObject CreateWithPublicKey(string base64PublicKey, int? keySize = null)
        {
            var cryptObject = new NeptuneCryptObject(keySize ?? _defaultKeySize);
            cryptObject.ImportPublicKeyBase64(base64PublicKey);
            return cryptObject;
        }

        /// <summary>
        /// Creates a NeptuneCryptObject with a private key imported from Base64
        /// </summary>
        /// <param name="base64PrivateKey">The private key in Base64 format</param>
        /// <param name="keySize">Key size in bits, or null to use the default</param>
        /// <returns>A new NeptuneCryptObject with the imported private key</returns>
        public NeptuneCryptObject CreateWithPrivateKey(string base64PrivateKey, int? keySize = null)
        {
            var cryptObject = new NeptuneCryptObject(keySize ?? _defaultKeySize);
            cryptObject.ImportPrivateKeyBase64(base64PrivateKey);
            return cryptObject;
        }

        /// <summary>
        /// Signs a message with one NeptuneCryptObject and verifies it with another
        /// </summary>
        /// <param name="message">The message to sign and verify</param>
        /// <param name="signer">The NeptuneCryptObject with the private key for signing</param>
        /// <param name="verifier">The NeptuneCryptObject with the public key for verification</param>
        /// <returns>True if signature is valid, false otherwise</returns>
        public bool SignAndVerify(string message, NeptuneCryptObject signer, NeptuneCryptObject verifier)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            if (signer == null)
                throw new ArgumentNullException(nameof(signer));

            if (verifier == null)
                throw new ArgumentNullException(nameof(verifier));

            // Sign with the signer's private key
            string signature = signer.SignMessage(message);

            // Verify with the verifier's public key
            return verifier.VerifySignature(message, signature);
        }

        /// <summary>
        /// Encrypts a message with the recipient's public key and decrypts it with the recipient's private key
        /// </summary>
        /// <param name="message">The message to encrypt and decrypt</param>
        /// <param name="sender">The NeptuneCryptObject with the recipient's public key</param>
        /// <param name="recipient">The NeptuneCryptObject with the recipient's private key</param>
        /// <returns>The decrypted message, which should match the original</returns>
        public string EncryptAndDecrypt(string message, NeptuneCryptObject sender, NeptuneCryptObject recipient)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (recipient == null)
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            // Encrypt with the recipient's public key
            string encrypted = sender.EncryptMessage(message);

            // Decrypt with the recipient's private key
            return recipient.DecryptMessage(encrypted);
        }
    }
