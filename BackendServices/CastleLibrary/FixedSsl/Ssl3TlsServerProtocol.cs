using Org.BouncyCastle.Tls;

namespace CastleLibrary.FixedSsl
{
    public class Ssl3TlsServerProtocol : TlsServerProtocol
    {
        private readonly Ssl3TlsServer _server;

        public Ssl3TlsServerProtocol(BCSSLCertificate certificate, Stream stream)
            : this(certificate, stream, stream) { }

        public Ssl3TlsServerProtocol(
            BCSSLCertificate certificate,
            Stream inputStream,
            Stream outputStream
        )
            : base(inputStream, outputStream)
        {
            _server = new Ssl3TlsServer(certificate);
            Accept(_server);
        }

        /// <summary>
        /// Ovverride server hello generator which allows downgrading used server tls version.
        /// The selected version is the max TLS version that supports both clients.
        /// </summary>
        protected override ServerHello GenerateServerHello(
            ClientHello clientHello,
            HandshakeMessageInput clientHelloMessage
        )
        {
            ProtocolVersion clientVersion = clientHello.Version;
            ProtocolVersion[] clientSupportedVersions =
                TlsExtensionsUtilities.GetSupportedVersionsExtensionClient(clientHello.Extensions);
            if (clientSupportedVersions == null)
            {
                if (clientVersion.IsLaterVersionOf(ProtocolVersion.TLSv12))
                    clientVersion = ProtocolVersion.TLSv12;

                clientSupportedVersions = clientVersion.DownTo(ProtocolVersion.SSLv3);
            }

            ProtocolVersion[] serverSupportedProtocols = Ssl3TlsServer.SupportedProtocols ?? [];
            ProtocolVersion negotiatedVersion = _server.GetServerVersion();

            // Choosing the max version that is supported by both client and server
            while (serverSupportedProtocols.Length > 0)
            {
                ProtocolVersion serverMaxVersion = ProtocolVersion.GetLatestTls(
                    serverSupportedProtocols
                );

                if (clientSupportedVersions.Contains(serverMaxVersion))
                {
                    negotiatedVersion = serverMaxVersion;
                    break;
                }

                serverSupportedProtocols = RemoveProtocolVersion(
                    serverSupportedProtocols,
                    serverMaxVersion
                );
            }

            _server.ServerVersion = negotiatedVersion;

            // now base GenerateServerHello will not throw an exception if the server version is not supported by the client, but it will use the common max tls version
            return base.GenerateServerHello(clientHello, clientHelloMessage);
        }

        private static ProtocolVersion[] RemoveProtocolVersion(
            ProtocolVersion[] versions,
            ProtocolVersion version
        )
        {
            return [.. versions.Where(v => v != version)];
        }
    }
}
