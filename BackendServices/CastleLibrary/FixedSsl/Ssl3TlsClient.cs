using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;

namespace FixedSsl.ProtoSSL;

public class Ssl3TlsClient : DefaultTlsClient
{
    private readonly TlsAuthentication _tlsAuth;
    public readonly BcTlsCrypto _crypto;

    public Ssl3TlsClient(BcTlsCrypto crypto, TlsAuthentication auth) : base(crypto)
    {
        _crypto = crypto;
        _tlsAuth = auth;
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

    public override ProtocolVersion[] GetProtocolVersions()
    {
        return _supportedVersions;
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

    public override TlsAuthentication GetAuthentication()
    {
        return _tlsAuth;
    }
}