using CastleLibrary.FixedSsl;
using Org.Mentalis.Security.Certificates;
using Org.Mentalis.Security.Ssl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FixedSsl
{
    public static class SslSocket
    {
        static SslSocket()
        {
            // TLS1.3 is only compatible with Windows 10 and Windows server 2019, for now I simply allow TLS1.2 to maintain compatibility, enable yourself if there is a need for 1.3 .
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 /*| SecurityProtocolType.Tls13*/;
        }

        private const int SSLv2 = 0x0002;  // SSL 2.0
        private const int SSLv3 = 0x0300;  // SSL 3.0
        private const int TLSv1 = 0x0301;  // TLS 1.0

        private static readonly SecureProtocol legacyProtocols = SecureProtocol.Ssl3 | SecureProtocol.Tls1;

        public static async Task<Stream> AuthenticateAsServerAsync(SslProtocols protocols, Socket socket, X509Certificate2 certificate, bool forceSsl, bool ownSocket)
        {
            // no certificate, no ssl
            if (certificate == null)
                return new NetworkStream(socket, true);

            // content type - 1 byte
            // version - 2 bytes
            // length - 2 bytes

            // total 5 bytes

            byte[] header = new byte[5];
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            int received = await socket.ReceiveAsync(header, SocketFlags.Peek).ConfigureAwait(false);
#else
            int received = socket.Receive(header, SocketFlags.Peek);
#endif
            if (received != 5)
                return null;

            bool ssl = header[0] == 0x16; // content type needs to be handshake (0x16)
            bool sslV2 = (header[0] & 0x80) != 0 || header[0] == 0x80; // SSLv2 Client Hello indicator

            if (!ssl && !sslV2)
            {
                if (forceSsl)
                    return null;
                return new NetworkStream(socket, ownSocket);
            }

            int totalLength = 0;
            byte[] clientHello = null;

            if (ssl)
            {
                // TLS: header[3..4] = record length
                if (received < 5)
                    return null;

                int recordLength = (header[3] << 8) | header[4];
                totalLength = 5 + recordLength;

                clientHello = new byte[totalLength];
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                received = await socket.ReceiveAsync(clientHello, SocketFlags.Peek).ConfigureAwait(false);
#else
                received = socket.Receive(clientHello, SocketFlags.Peek);
#endif
                if (received < totalLength)
                    return null;

                // handshake type needs to be client hello (0x01)
                if (clientHello[5] != 0x01)
                {
                    if (forceSsl)
                        return null;
                    return new NetworkStream(socket, ownSocket);
                }
            }
            else if (sslV2)
            {
                // SSLv2 header: first 2 bytes = 15-bit length
                int v2Length = ((header[0] & 0x7F) << 8) | header[1];
                totalLength = v2Length + 2; // SSLv2 header length

                clientHello = new byte[totalLength];
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                received = await socket.ReceiveAsync(clientHello, SocketFlags.Peek).ConfigureAwait(false);
#else
                received = socket.Receive(clientHello, SocketFlags.Peek);
#endif
                if (received < totalLength)
                    return null;

                // SSLv2 Client Hello validation
                if (clientHello[2] != 0x01) // Message type must be Client Hello
                {
                    if (forceSsl)
                        return null;
                    return new NetworkStream(socket, ownSocket);
                }
            }
            else
            {
                if (forceSsl)
                    return null;
                return new NetworkStream(socket, ownSocket);
            }

            int parseResult = TlsParser.ParseTlsHeader(clientHello, out string hostname, out _, out int maxSslVersion, out List<int> versions);

            var allowedProtocols = protocols.GetEnabledProtocols();

            // Microsoft doesn't like our FESL exploit, so we fallback to a older crypto supported by Mentalis if that's the case.
            if (
#pragma warning disable
                    (allowedProtocols.Contains(SslProtocols.Ssl3) || allowedProtocols.Contains(SslProtocols.Tls)) &&
                    (
                        maxSslVersion == SSLv3 ||
                        maxSslVersion == TLSv1 ||
                        (!certificate.Verify() && versions.Any(v => v == SSLv3 || v == TLSv1))
                    )
                )
                return new SecureNetworkStream(new SecureSocket(socket, new SecurityOptions(legacyProtocols, new Certificate(certificate), ConnectionEnd.Server)), true);
            else if (allowedProtocols.Contains(SslProtocols.Ssl2) && maxSslVersion == SSLv2)
#pragma warning restore
            {
                CustomLogger.LoggerAccessor.LogWarn($"[SslSocket] - Client tried to initialize a SSLv2 connection at {DateTime.Now}, invalidating the request...");
                return null;
            }

            SslStream sslStream = new SslStream(new NetworkStream(socket, ownSocket), false);

            await sslStream.AuthenticateAsServerAsync(certificate).ConfigureAwait(false);
            return sslStream;
        }

        public static Stream AuthenticateAsServer(
             Socket socket,
             SslServerAuthenticationOptions authOptions,
             bool forceSsl,
             bool ownSocket,
             out X509Certificate2 clientCertificate,
             out int[] clientCertificateErrors
             )
        {
            clientCertificate = null;
            clientCertificateErrors = null;

            // no certificate, no ssl
            if (authOptions == null)
                return new NetworkStream(socket, ownSocket);

            // content type - 1 byte
            // version - 2 bytes
            // length - 2 bytes

            // total 5 bytes

            byte[] header = new byte[5];
            int received = socket.Receive(header, SocketFlags.Peek);
            if (received != 5)
                return null;

            bool ssl = header[0] == 0x16; // content type needs to be handshake (0x16)
            bool sslV2 = (header[0] & 0x80) != 0 || header[0] == 0x80; // SSLv2 Client Hello indicator

            if (!ssl && !sslV2)
            {
                if (forceSsl)
                    return null;
                return new NetworkStream(socket, ownSocket);
            }

            int totalLength = 0;
            byte[] clientHello = null;

            if (ssl)
            {
                // TLS: header[3..4] = record length
                if (received < 5)
                    return null;

                int recordLength = (header[3] << 8) | header[4];
                totalLength = 5 + recordLength;

                clientHello = new byte[totalLength];

                received = socket.Receive(clientHello, SocketFlags.Peek);

                if (received < totalLength)
                    return null;

                // handshake type needs to be client hello (0x01)
                if (clientHello[5] != 0x01)
                {
                    if (forceSsl)
                        return null;
                    return new NetworkStream(socket, ownSocket);
                }
            }
            else if (sslV2)
            {
                // SSLv2 header: first 2 bytes = 15-bit length
                int v2Length = ((header[0] & 0x7F) << 8) | header[1];
                totalLength = v2Length + 2; // SSLv2 header length

                clientHello = new byte[totalLength];

                received = socket.Receive(clientHello, SocketFlags.Peek);

                if (received < totalLength)
                    return null;

                // SSLv2 Client Hello validation
                if (clientHello[2] != 0x01) // Message type must be Client Hello
                {
                    if (forceSsl)
                        return null;
                    return new NetworkStream(socket, ownSocket);
                }
            }
            else
            {
                if (forceSsl)
                    return null;
                return new NetworkStream(socket, ownSocket);
            }

            int parseResult = TlsParser.ParseTlsHeader(clientHello, out string hostname, out _, out int maxSslVersion, out List<int> versions);

            X509Certificate2 certificate = (X509Certificate2)authOptions.ServerCertificateSelectionCallback?.Invoke(socket, hostname);
            if (certificate == null)
            {
                if (forceSsl)
                    return null;
                return new NetworkStream(socket, ownSocket);
            }

            var allowedProtocols = authOptions.EnabledSslProtocols.GetEnabledProtocols();

            // Microsoft doesn't like our FESL exploit, so we fallback to a older crypto supported by Mentalis if that's the case.
            if (
#pragma warning disable
                    (allowedProtocols.Contains(SslProtocols.Ssl3) || allowedProtocols.Contains(SslProtocols.Tls)) &&
                    (
                        maxSslVersion == SSLv3 ||
                        maxSslVersion == TLSv1 ||
                        (!certificate.Verify() && versions.Any(v => v == SSLv3 || v == TLSv1))
                    )
                )
            {
                SecureSocket sock = new SecureSocket(socket, new SecurityOptions(legacyProtocols, new Certificate(certificate), ConnectionEnd.Server));

                // Only fills the client certificate since for now, I have no idea how to extract cert related failures.
                clientCertificate = new X509Certificate2(sock.RemoteCertificate.UnderlyingCert.GetRawCertData());

                // Idea: using a lookup based on the client endpoint and certificate to return DNAS certificates on the fly.

                return new SecureNetworkStream(sock, true);
            }
            else if (allowedProtocols.Contains(SslProtocols.Ssl2) && maxSslVersion == SSLv2)
#pragma warning restore
            {
                CustomLogger.LoggerAccessor.LogWarn($"[SslSocket] - Client tried to initialize a SSLv2 connection at {DateTime.Now}, invalidating the request...");
                return null;
            }

            int[] clientCertErr = null;
            X509Certificate2 clientCert = null;

            SslStream sslStream = new SslStream(new NetworkStream(socket, ownSocket), false, (t, c, ch, e) =>
            {
                if (c == null)
                    return true;

                X509Certificate2 c2 = c as X509Certificate2;
                c2 ??= new X509Certificate2(c.GetRawCertData());

                clientCert = c2;
                clientCertErr = new int[] { (int)e };
                return true;
            });

            // Shortcut
            authOptions.ServerCertificateSelectionCallback = (sender, host) => certificate;

            clientCertificate = clientCert;
            clientCertificateErrors = clientCertErr;

            sslStream.AuthenticateAsServer(authOptions);
            return sslStream;
        }

        public static IAsyncResult BeginAuthenticateAsServer(SslProtocols protocols, Socket socket, X509Certificate2 certificate, bool forceSsl, bool ownSocket, AsyncCallback callback, object state)
        {
            return AuthenticateAsServerAsync(protocols, socket, certificate, forceSsl, ownSocket).AsApm(callback, state);
        }

        public static IAsyncResult BeginAuthenticateAsServer(
            Socket socket,
            SslServerAuthenticationOptions authOptions,
            bool forceSsl,
            bool ownSocket,
            AsyncCallback callback,
            object state,
            out X509Certificate2 clientCertificate,
            out int[] clientCertificateErrors)
        {
            X509Certificate2 localClientCert = null;
            int[] localCertErrors = null;

            var task = Task.Run(() =>
            {
                return AuthenticateAsServer(
                    socket,
                    authOptions,
                    forceSsl,
                    ownSocket,
                    out localClientCert,
                    out localCertErrors);
            });

            clientCertificate = localClientCert;
            clientCertificateErrors = localCertErrors;

            return task.AsApm(callback, state);
        }

        public static Stream EndAuthenticateAsServer(IAsyncResult result)
        {
            return ((Task<Stream>)result).Result;
        }

        #region Helpers
        private static IAsyncResult AsApm<T>(this Task<T> task,
                                    AsyncCallback callback,
                                    object state)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            var tcs = new TaskCompletionSource<T>(state);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null && t.Exception.InnerExceptions != null)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(t.Result);

                if (callback != null)
                    callback(tcs.Task);
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        #endregion
    }
}