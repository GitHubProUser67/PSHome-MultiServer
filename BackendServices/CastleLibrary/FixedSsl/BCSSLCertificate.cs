using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace FixedSsl
{
    // From: https://github.com/Aim4kill/BlazeSDK/blob/master/ProtoFire/Tls/ProtoSSLCertificate.cs
    public class BCSSLCertificate
    {
        public AsymmetricKeyParameter PrivateKey { get; }
        public Certificate Certificate { get; }

        public BCSSLCertificate(AsymmetricKeyParameter privateKey, Certificate certificate)
        {
            PrivateKey = privateKey;
            Certificate = certificate;
        }

        public BCSSLCertificate(X509Certificate2 certificate)
        {
            AsymmetricAlgorithm? privateKey = GetPrivateKey(certificate);
            if (privateKey == null)
                throw new ArgumentException("[BCSSLCertificate] - Certificate does not contain a private key");

            try
            {
                PrivateKey = DotNetUtilities.GetKeyPair(privateKey).Private;
            }
            catch (CryptographicException exception)
            {
                throw new ArgumentException("[BCSSLCertificate] - Invalid certificate private key or private key is not exportable (missing X509KeyStorageFlags.Exportable flag).", exception);
            }

            Certificate = new Certificate(new TlsCertificate[] { new BcTlsCertificate(new BcTlsCrypto(new SecureRandom()), DotNetUtilities.FromX509Certificate(certificate).CertificateStructure) });
        }

        public static BCSSLCertificate FromX509Certificate2(X509Certificate2 certificate) => new BCSSLCertificate(certificate);

        public X509Certificate2 AsX509Certificate2()
        {
            // TODO
            throw new NotImplementedException();
        }

        public static AsymmetricAlgorithm GetPrivateKey(X509Certificate2 certificate)
        {
            // X509Certificate2 has PrivateKey property, but it is deprecated.
            // This function has been created to avoid getting warning about it.

            if (!certificate.HasPrivateKey)
                return null;

            RSA rsa = certificate.GetRSAPrivateKey();
            if (rsa != null)
                return rsa;

            DSA dsa = certificate.GetDSAPrivateKey();
            if (dsa != null)
                return dsa;

            ECDsa ecdsa = certificate.GetECDsaPrivateKey();
            if (ecdsa != null)
                return ecdsa;

            ECDiffieHellman ecdh = certificate.GetECDiffieHellmanPrivateKey();
            if (ecdh != null)
                return ecdh;

            throw new NotSupportedException("[BCSSLCertificate] - GetPrivateKey: Key algorithm not supported");
        }

        public static implicit operator BCSSLCertificate(X509Certificate2 certificate) => new BCSSLCertificate(certificate);
    }
}
