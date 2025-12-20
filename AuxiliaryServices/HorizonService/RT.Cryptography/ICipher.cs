namespace Horizon.RT.Cryptography
{
    /// <summary>
    /// The highest 3 bits of the hash.
    /// Denotes which key is used to encrypt/decrypt the respective message.
    /// </summary>
    public enum CipherContext
    {
        ID_00,
        RC_SERVER_SESSION,
        ID_02,
        RC_CLIENT_SESSION,
        ID_04,
        ID_05,
        ID_06,
        RSA_AUTH
    }

    public interface ICipher
    {
        /// <summary>
        /// Cipher context.
        /// </summary>
        CipherContext Context { get; }

        /// <summary>
        /// Decrypts the cipher text with the hash.
        /// Fails if the hash doesn't matched the plaintext.
        /// </summary>
        bool Decrypt(byte[] input, byte[] hash, out byte[] plain);

        /// <summary>
        /// Encrypts the given input buffer and returns the cipher and hash.
        /// </summary>
        bool Encrypt(byte[] input, out byte[] cipher, out byte[] hash);

        /// <summary>
        /// Computes the hash of the given input buffer.
        /// </summary>
        void Hash(byte[] input, out byte[] hash);

        /// <summary>
        /// Whether or not the input hash sequence is valid.
        /// </summary>
        bool IsHashValid(byte[] hash);

        /// <summary>
        /// Returns the public key.
        /// </summary>
        byte[] GetPublicKey();
    }
}