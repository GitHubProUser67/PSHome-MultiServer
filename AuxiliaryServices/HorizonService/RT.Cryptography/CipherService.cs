using CustomLogger;
using System.Collections.Generic;

namespace Horizon.RT.Cryptography
{
    public class CipherService
    {
        private ICipherFactory _factory = null;
        private Dictionary<CipherContext, ICipher> _ciphers = new Dictionary<CipherContext, ICipher>();

        public bool EnableEncryption { get; set; } = true;

        public CipherService(ICipherFactory factory)
        {
            _factory = factory;
        }

        public void GenerateCipher(CipherContext context)
        {
            if (!_ciphers.ContainsKey(context))
                _ciphers.Add(context, _factory.CreateNew(context));
            else
                _ciphers[context] = _factory.CreateNew(context);
        }

        public void GenerateCipher(CipherContext context, byte[] publicKey)
        {
            if (!_ciphers.ContainsKey(context))
                _ciphers.Add(context, _factory.CreateNew(context, publicKey));
            else
                _ciphers[context] = _factory.CreateNew(context, publicKey);
        }

        public void GenerateCipher(RSA.RsaKeyPair rsaKeyPair)
        {
            CipherContext context = CipherContext.RSA_AUTH;
            if (!_ciphers.ContainsKey(context))
                _ciphers.Add(context, _factory.CreateNew(rsaKeyPair));
            else
                _ciphers[context] = _factory.CreateNew(rsaKeyPair);
        }

        public ICipher GetCipher(CipherContext context)
        {
            ICipher cipher = _ciphers[context];
            if (cipher == null)
            {
                LoggerAccessor.LogError($"The CipherContext {context} does not have a cipher associated with it.");
                return null;
            }

            return cipher;
        }

        public void SetCipher(CipherContext context, ICipher cipher)
        {
            if (!_ciphers.ContainsKey(context))
                _ciphers.Add(context, cipher);
            else
                _ciphers[context] = cipher;
        }

        public bool HasKey(CipherContext context)
        {
            return _ciphers.TryGetValue(context, out ICipher value) && value != null;
        }

        public byte[] GetPublicKey(CipherContext context)
        {
            ICipher cipher = _ciphers[context];
            if (cipher == null)
            {
                LoggerAccessor.LogError($"The CipherContext {context} does not have a cipher associated with it.");
                return null;
            }

            return cipher.GetPublicKey();
        }

        public bool Encrypt(CipherContext context, byte[] input, out byte[] cipher, out byte[] hash)
        {
            cipher = null;
            hash = null;
            if (!EnableEncryption || !_ciphers.TryGetValue(context, out ICipher c) || c == null)
                return false;

            return c.Encrypt(input, out cipher, out hash);
        }

        public bool Decrypt(byte[] input, byte[] hash, out byte[] plain)
        {
            CipherContext cipherContext = (CipherContext)(hash[3] >> 5);
            return Decrypt(cipherContext, input, hash, out plain);
        }

        public bool Decrypt(CipherContext context, byte[] input, byte[] hash, out byte[] plain)
        {
            if (!_ciphers.TryGetValue(context, out var cipher) || cipher == null)
            {
                LoggerAccessor.LogError($"The CipherContext {context} does not have a cipher associated with it.");
                plain = null;
                return false;
            }

            return cipher.Decrypt(input, hash, out plain);
        }

    }
}
