using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System.Linq;

namespace FixedSsl;

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

    public static readonly int[] AESCipherSuites = new int[]
    {
        CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA,
        CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA,
    };

    public static readonly int[] RC4CipherSuites = new int[]
    {
        CipherSuite.TLS_RSA_WITH_RC4_128_MD5,
        CipherSuite.TLS_RSA_WITH_RC4_128_SHA
    };

    private static readonly ProtocolVersion[] _supportedVersions = new ProtocolVersion[]
    {
        ProtocolVersion.SSLv3,
        ProtocolVersion.TLSv10,
        ProtocolVersion.TLSv11
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
        return AESCipherSuites.Concat(RC4CipherSuites).ToArray();
    }

    protected override int[] GetSupportedCipherSuites()
    {
        return AESCipherSuites.Concat(RC4CipherSuites).ToArray();
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