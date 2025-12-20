using System;
using System.Security.Cryptography;

namespace FixedSsl
{
    // Allows to use booth the older and newer services (The provider can fail on some certificates)
    public class RSACryptoServiceProviderWrapper
    {
        private readonly RSACryptoServiceProvider _rsaCsp = null; // Classic RSACryptoServiceProvider
        private readonly RSA _rsa = null; // Modern RSA (non-exportable possible)

        public RSACryptoServiceProvider RsaCsp
        {
            get
            {
                return _rsaCsp;
            }
        }

        public RSA Rsa
        {
            get
            {
                return _rsa;
            }
        }

        public RSACryptoServiceProviderWrapper(RSA rsa)
        {
            if (rsa == null)
                throw new ArgumentNullException(nameof(rsa));

            if (rsa is RSACryptoServiceProvider rsaCsp)
                _rsaCsp = rsaCsp;
            else
                _rsa = rsa;
        }

        public void Clear()
        {
            _rsaCsp?.Clear();
            _rsa?.Clear();
        }

        public byte[] Encrypt(byte[] data, bool useOaepPadding)
        {
            if (_rsaCsp != null)
                return _rsaCsp.Encrypt(data, useOaepPadding);

            // Modern RSA path
            return _rsa.Encrypt(data, useOaepPadding ? RSAEncryptionPadding.OaepSHA1 : RSAEncryptionPadding.Pkcs1);
        }

        public byte[] Decrypt(byte[] data, bool useOaepPadding)
        {
            if (_rsaCsp != null)
                return _rsaCsp.Decrypt(data, useOaepPadding);

            return _rsa.Decrypt(data, useOaepPadding ? RSAEncryptionPadding.OaepSHA1 : RSAEncryptionPadding.Pkcs1);
        }

        public byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
        {
            if (padding != RSASignaturePadding.Pkcs1)
                throw new NotSupportedException("[RSACryptoServiceProviderWrapper] - RSACryptoServiceProvider only supports PKCS#1 padding.");

            if (_rsaCsp != null)
                return _rsaCsp.SignData(data, hashAlgorithm.Name);

            return _rsa.SignData(data, hashAlgorithm, padding);
        }

        public bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
        {
            if (padding != RSASignaturePadding.Pkcs1)
                throw new NotSupportedException("[RSACryptoServiceProviderWrapper] - RSACryptoServiceProvider only supports PKCS#1 padding.");

            if (_rsaCsp != null)
                return _rsaCsp.VerifyData(data, hashAlgorithm.Name, signature);

            return _rsa.VerifyData(data, signature, hashAlgorithm, padding);
        }
    }
}
