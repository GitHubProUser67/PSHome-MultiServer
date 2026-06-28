using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;

namespace CastleLibrary.FixedSsl
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
            var privateKey = GetPrivateKey(certificate) ?? throw new ArgumentException(
                    "[BCSSLCertificate] - Certificate does not contain a private key"
                );
            try
            {
                PrivateKey = DotNetUtilities.GetKeyPair(privateKey).Private;
            }
            catch (CryptographicException exception)
            {
                throw new ArgumentException(
                    "[BCSSLCertificate] - Invalid certificate private key or private key is not exportable (missing X509KeyStorageFlags.Exportable flag).",
                    exception
                );
            }

            Certificate = new Certificate([
                new BcTlsCertificate(
                    new BcTlsCrypto(new SecureRandom()),
                    DotNetUtilities.FromX509Certificate(certificate).CertificateStructure
                ),
            ]);
        }

        public static AsymmetricAlgorithm GetPrivateKey(X509Certificate2 certificate)
        {
            // X509Certificate2 has PrivateKey property, but it is deprecated.
            // This function has been created to avoid getting warning about it.

            if (!certificate.HasPrivateKey)
                return null;

            var rsa = certificate.GetRSAPrivateKey();
            if (rsa != null)
                return rsa;

            var dsa = certificate.GetDSAPrivateKey();
            if (dsa != null)
                return dsa;

            var ecdsa = certificate.GetECDsaPrivateKey();
            if (ecdsa != null)
                return ecdsa;

            var ecdh = certificate.GetECDiffieHellmanPrivateKey();
            return ecdh != null
                ? (AsymmetricAlgorithm)ecdh
                : throw new NotSupportedException(
                    "[BCSSLCertificate] - GetPrivateKey: Key algorithm not supported"
                );
        }

        public static BCSSLCertificate FromX509Certificate2(X509Certificate2 certificate) =>
            new(certificate);

        public static implicit operator BCSSLCertificate(X509Certificate2 certificate) =>
            FromX509Certificate2(certificate);
    }
}
