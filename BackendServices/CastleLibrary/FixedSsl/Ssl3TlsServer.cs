using CastleLibrary.FixedSsl.Crypto;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;

namespace CastleLibrary.FixedSsl
{
    public class Ssl3TlsServer(BCSSLCertificate certificate) : DefaultTlsServer(_crypto)
    {
        private static readonly Rc4TlsCrypto _crypto =
#if DEBUG
        new(true)
#else
        new(false)
#endif
        ;
        private readonly BCSSLCertificate _certificate = certificate;

        public static ProtocolVersion[] SupportedProtocols => ProtoSSL.SupportedProtocols;
        public ProtocolVersion ServerVersion { get; internal set; } = ProtocolVersion.SSLv3; // Minimum version

        public override ProtocolVersion GetServerVersion() => ServerVersion;

        protected override ProtocolVersion[] GetSupportedVersions() => SupportedProtocols;

        public override int[] GetCipherSuites() => ProtoSSL.GetCipherSuites(GetSupportedVersions());

        protected override int[] GetSupportedCipherSuites() =>
            TlsUtilities.GetSupportedCipherSuites(_crypto, GetCipherSuites());

        protected override bool SelectCipherSuite(int cipherSuite)
        {
            int keyExchangeAlgorithm = TlsUtilities.GetKeyExchangeAlgorithm(cipherSuite);

            if (KeyExchangeAlgorithm.IsAnonymous(keyExchangeAlgorithm))
                return base.SelectCipherSuite(cipherSuite);

            if (keyExchangeAlgorithm != KeyExchangeAlgorithm.RSA)
                return false;

            return base.SelectCipherSuite(cipherSuite);
        }

        public override void NotifySecureRenegotiation(bool secureRenegotiation)
        {
            secureRenegotiation = true;
            base.NotifySecureRenegotiation(secureRenegotiation);
        }

        protected override TlsCredentialedDecryptor GetRsaEncryptionCredentials()
        {
            return new BcDefaultTlsCredentialedDecryptor(
                _crypto,
                _certificate.Certificate,
                _certificate.PrivateKey
            );
        }
    }
}
