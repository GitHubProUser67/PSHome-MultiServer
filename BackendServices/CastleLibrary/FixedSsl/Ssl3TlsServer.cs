using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;

namespace FixedSsl.ProtoSSL;

public class Ssl3TlsServer : DefaultTlsServer
{
    private readonly Certificate _serverCertificate;
    private readonly AsymmetricKeyParameter _serverPrivateKey;
    private readonly BcTlsCrypto _crypto;

    public Ssl3TlsServer(BcTlsCrypto crypto, Certificate serverCertificate, AsymmetricKeyParameter serverPrivateKey) : base(crypto)
    {
        _crypto = crypto;
        _serverCertificate = serverCertificate;
        _serverPrivateKey = serverPrivateKey;
    }

    private static readonly int[] _cipherSuites = new int[]
    {
        CipherSuite.TLS_RSA_WITH_RC4_128_MD5,
        CipherSuite.TLS_RSA_WITH_RC4_128_SHA
    };

    private static readonly ProtocolVersion[] _supportedVersions = new ProtocolVersion[]
    {
        ProtocolVersion.SSLv3,
        ProtocolVersion.TLSv10
    };

    public override ProtocolVersion GetServerVersion()
    {
        return _supportedVersions[0];
    }

    protected override ProtocolVersion[] GetSupportedVersions()
    {
        return _supportedVersions;
    }

    public override int[] GetCipherSuites()
    {
        return _cipherSuites;
    }

    protected override int[] GetSupportedCipherSuites()
    {
        return _cipherSuites;
    }

    public override void NotifySecureRenegotiation(bool secureRenegotiation)
    {
        if (!secureRenegotiation)
        {
            secureRenegotiation = true;
        }

        base.NotifySecureRenegotiation(secureRenegotiation);
    }

    protected override TlsCredentialedDecryptor GetRsaEncryptionCredentials()
    {
        return new BcDefaultTlsCredentialedDecryptor(_crypto, _serverCertificate, _serverPrivateKey);
    }
}