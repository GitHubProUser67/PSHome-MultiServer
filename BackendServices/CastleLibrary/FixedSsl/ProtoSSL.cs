using Org.BouncyCastle.Tls;

namespace CastleLibrary.FixedSsl
{
    public static class ProtoSSL
    {
        // These cipher suites are taken from EAWebkit's (15.1.6.0.5) protossl.c file + 2 really old cipher suites that some old games uses
        public static readonly (ProtocolVersion, int)[] CipherSuites =
        [
            /* TLS 1.3 */
            (ProtocolVersion.TLSv13, CipherSuite.TLS_AES_128_GCM_SHA256),
            (ProtocolVersion.TLSv13, CipherSuite.TLS_AES_256_GCM_SHA384),
            (ProtocolVersion.TLSv13, CipherSuite.TLS_CHACHA20_POLY1305_SHA256),
            /* TLS 1.2 AEAD */
            (ProtocolVersion.TLSv12, CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384),
            /* TLS 1.2 */
            (ProtocolVersion.TLSv12, CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256),
            (ProtocolVersion.TLSv12, CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256),
            /* TLS 1.0 */
            (ProtocolVersion.TLSv10, CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA),
            (ProtocolVersion.TLSv10, CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA),
            (ProtocolVersion.TLSv10, CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA),
            (ProtocolVersion.TLSv10, CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA),
            /* SSL 3.0 */
            (ProtocolVersion.SSLv3, CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA),
            (ProtocolVersion.SSLv3, CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA),
            (ProtocolVersion.SSLv3, CipherSuite.TLS_RSA_WITH_RC4_128_SHA),
            (ProtocolVersion.SSLv3, CipherSuite.TLS_RSA_WITH_RC4_128_MD5),
        ];

        public static readonly ProtocolVersion[] SupportedProtocols =
        [
            ProtocolVersion.SSLv3,
            ProtocolVersion.TLSv10,
            ProtocolVersion.TLSv11,
            ProtocolVersion.TLSv12,
            //Not supported
            //ProtocolVersion.TLSv13
        ];

        public static int[] GetCipherSuites(ProtocolVersion[] versions)
        {
            List<int> cipherSuites = [with(CipherSuites.Length)];

            foreach (ProtocolVersion version in versions)
                cipherSuites.AddRange(
                    CipherSuites.Where(x => x.Item1 == version).Select(x => x.Item2)
                );

            return [.. cipherSuites];
        }
    }
}
