using FixedSsl.Crypto;
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
            // Enables wildcards certificate support in WebClient.
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;

            // TLS1.3 is only compatible with Windows 10 and Windows server 2019, for now I simply allow TLS1.2 to maintain compatibility, enable yourself if there is a need for 1.3 .
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 /*| SecurityProtocolType.Tls13*/;
        }

        // Some domains are not valid anymore, but we need them, and know they aren't trapped websites...
        private static readonly string[] _invalidCNBypassList = new string[]
        {
            "s3.amazonaws.com"
        };

        private const int SSLv2 = 0x0002;  // SSL 2.0
        private const int SSLv3 = 0x0300;  // SSL 3.0
        private const int TLSv1 = 0x0301;  // TLS 1.0

        private static readonly Org.Mentalis.Security.Ssl.SecureProtocol legacyProtocols = Org.Mentalis.Security.Ssl.SecureProtocol.Ssl3 | Org.Mentalis.Security.Ssl.SecureProtocol.Tls1;

        public static List<string> ClientCertificateCNBypassList = new List<string>()
        {
            // Add server CN in which we don't want to validate client certificates.
        };

        public static async Task<Stream> AuthenticateAsServerAsync(SslProtocols protocols, Socket socket, X509Certificate2 certificate, bool forceSsl, bool ownSocket)
        {
            // no certificate, no ssl
            if (certificate == null)
                return new NetworkStream(socket, ownSocket);

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

            int parseResult = TlsParser.ParseTlsHeader(clientHello, out string hostname, out bool isSslV2, out int maxSslVersion, out List<int> versions, out List<int> cipherSuites);
#if DEBUG
            CustomLogger.LoggerAccessor.LogInfo($"[SslSocket] - ClientHello (status:{parseResult}) params: Hostname:{hostname} IsSSLV2:{isSslV2} MaxSSLVersion:{maxSslVersion} Versions:{(versions.Count > 0 ? string.Join(", ", versions.Select(v => $"0x{v:X4}")) : "none")} CipherSuites:{(cipherSuites.Count > 0 ? string.Join(", ", cipherSuites.Select(cs => $"0x{cs:X4}")) : "none")}");
#endif
            var allowedProtocols = protocols.GetEnabledProtocols();
#pragma warning disable            // Microsoft doesn't like our FESL exploit, so we fallback to a older crypto supported by Mentalis if that's the case.
            if (
                    (allowedProtocols.Contains(SslProtocols.Ssl3) || allowedProtocols.Contains(SslProtocols.Tls)) &&
                    (
                        maxSslVersion == SSLv3 ||
                        maxSslVersion == TLSv1 ||
                        (!certificate.Verify() && versions.Any(v => v == SSLv3 || v == TLSv1))
                    )
                )
            {
#if !FORCE_MENTALIS_SSL_SERVER
                if (!isSslV2 && (cipherSuites.Exists(c => Ssl3TlsServer.AESCipherSuites.Contains(c)) || cipherSuites.Exists(c => Ssl3TlsServer.RC4CipherSuites.Contains(c))))
                {
                    BCSSLCertificate bcCertificate = null;

                    try
                    {
                        bcCertificate = certificate;
                    }
                    catch (ArgumentException)
                    {
                        // Fallback to Mentalis.
                    }

                    if (bcCertificate != null)
                    {
                        Ssl3TlsServer connTls = new(
#if DEBUG
                        new Rc4TlsCrypto(true)
#else
                        new Rc4TlsCrypto(false)
#endif
                        , bcCertificate.Certificate, bcCertificate.PrivateKey);
                        Org.BouncyCastle.Tls.TlsServerProtocol serverProtocol = new(new NetworkStream(socket, ownSocket));

                        serverProtocol.Accept(connTls);

                        return serverProtocol.Stream;
                    }
                }
#endif
                return new Org.Mentalis.Security.Ssl.SecureNetworkStream(new Org.Mentalis.Security.Ssl.SecureSocket(socket, new Org.Mentalis.Security.Ssl.SecurityOptions(legacyProtocols, new Org.Mentalis.Security.Certificates.Certificate(certificate), Org.Mentalis.Security.Ssl.ConnectionEnd.Server)), ownSocket);
            }
            else if (allowedProtocols.Contains(SslProtocols.Ssl2) && maxSslVersion == SSLv2)
#pragma warning restore
                throw new NotSupportedException($"[SslSocket] - Client tried to initialize a SSLv2 connection which is not supported yet, invalidating the request...");

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

            int parseResult = TlsParser.ParseTlsHeader(clientHello, out string hostname, out bool isSslV2, out int maxSslVersion, out List<int> versions, out List<int> cipherSuites);
#if DEBUG
            CustomLogger.LoggerAccessor.LogInfo($"[SslSocket] - ClientHello (status:{parseResult}) params: Hostname:{hostname} IsSSLV2:{isSslV2} MaxSSLVersion:{maxSslVersion} Versions:{(versions.Count > 0 ? string.Join(", ", versions.Select(v => $"0x{v:X4}")) : "none")} CipherSuites:{(cipherSuites.Count > 0 ? string.Join(", ", cipherSuites.Select(cs => $"0x{cs:X4}")) : "none")}");
#endif
            X509Certificate2 certificate = (X509Certificate2)authOptions.ServerCertificateSelectionCallback?.Invoke(socket, hostname);
            if (certificate == null)
            {
                if (forceSsl)
                    return null;
                return new NetworkStream(socket, ownSocket);
            }

            var allowedProtocols = authOptions.EnabledSslProtocols.GetEnabledProtocols();
#pragma warning disable
            // Microsoft doesn't like our FESL exploit, so we fallback to a older crypto supported by Mentalis if that's the case.
            if (
                    (allowedProtocols.Contains(SslProtocols.Ssl3) || allowedProtocols.Contains(SslProtocols.Tls)) &&
                    (
                        maxSslVersion == SSLv3 ||
                        maxSslVersion == TLSv1 ||
                        (!certificate.Verify() && versions.Any(v => v == SSLv3 || v == TLSv1))
                    )
                )
            {
#if !FORCE_MENTALIS_SSL_SERVER
                if (!isSslV2 && (cipherSuites.Exists(c => Ssl3TlsServer.AESCipherSuites.Contains(c)) || cipherSuites.Exists(c => Ssl3TlsServer.RC4CipherSuites.Contains(c))))
                {
                    BCSSLCertificate bcCertificate = null;

                    try
                    {
                        bcCertificate = certificate;
                    }
                    catch (ArgumentException)
                    {
                        // Fallback to Mentalis.
                    }

                    if (bcCertificate != null)
                    {
                        Ssl3TlsServer connTls = new(
#if DEBUG
                        new Rc4TlsCrypto(true)
#else
                        new Rc4TlsCrypto(false)
#endif
                        , bcCertificate.Certificate, bcCertificate.PrivateKey);
                        Org.BouncyCastle.Tls.TlsServerProtocol serverProtocol = new(new NetworkStream(socket, ownSocket));

                        serverProtocol.Accept(connTls);

                        return serverProtocol.Stream;
                    }
                }
#endif
                return new Org.Mentalis.Security.Ssl.SecureNetworkStream(new Org.Mentalis.Security.Ssl.SecureSocket(socket, new Org.Mentalis.Security.Ssl.SecurityOptions(legacyProtocols, new Org.Mentalis.Security.Certificates.Certificate(certificate), Org.Mentalis.Security.Ssl.ConnectionEnd.Server)), ownSocket);
            }
            else if (allowedProtocols.Contains(SslProtocols.Ssl2) && maxSslVersion == SSLv2)
#pragma warning restore
                throw new NotSupportedException($"[SslSocket] - Client tried to initialize a SSLv2 connection which is not supported yet, invalidating the request...");

            int[] clientCertErr = null;
            X509Certificate2 clientCert = null;
            bool bypassClientCertValidation = ClientCertificateCNBypassList.Contains(hostname);

            if (bypassClientCertValidation || authOptions.RemoteCertificateValidationCallback == null)
            {
                authOptions.RemoteCertificateValidationCallback = (t, c, ch, e) =>
                {
                    if (c == null)
                        return true;

                    X509Certificate2 c2 = c as X509Certificate2;
                    c2 ??= new X509Certificate2(c.GetRawCertData());

                    clientCert = c2;
                    clientCertErr = new int[] { (int)e };
                    return true;
                };
            }

            SslStream sslStream = new SslStream(new NetworkStream(socket, ownSocket), false, authOptions.RemoteCertificateValidationCallback);

            // Shortcut
            authOptions.ServerCertificateSelectionCallback = (sender, host) => certificate;

            // Avoids the client cert popup if we don't need it.
            if (authOptions.ClientCertificateRequired && bypassClientCertValidation)
                authOptions.ClientCertificateRequired = false;

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

        /// <summary>
        /// Certificate validation callback, fixes the ndreams objs endpoint in ApacheNet (usage of wildcard amazon certs).
        /// </summary>
        private static bool ValidateRemoteCertificate(
           object sender,
           X509Certificate cert,
           X509Chain chain,
           SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;

            // Extract CN or SAN hostnames
            var certName = cert?.Subject?.Split(',')
                .Select(s => s.Trim())
                .FirstOrDefault(s => s.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
                ?.Substring(3);

            if (string.IsNullOrEmpty(certName))
                return false;

            // Get requested host from request
            string requestHost = null;
            if (sender is HttpWebRequest req)
                requestHost = req.RequestUri.Host;
            else if (sender is SslStream sslStream && !string.IsNullOrEmpty(sslStream.TargetHostName))
                requestHost = sslStream.TargetHostName;

            if (string.IsNullOrEmpty(requestHost))
                return false;

            // Custom multi-level dot wildcard check
            if (IsDotWildcardMatch(certName, requestHost))
                return true;
            else if (_invalidCNBypassList.Contains(certName))
                return true;

            CustomLogger.LoggerAccessor.LogError("[SslSocket] - ValidateRemoteCertificate: X509Certificate [{0}] Policy Error: '{1}'",
                cert.Subject,
                errors.ToString());

            return false;
        }

        private static bool IsDotWildcardMatch(string pattern, string host)
        {
            if (string.Equals(pattern, host, StringComparison.OrdinalIgnoreCase))
                return true;

            // If pattern starts with "*.", allow multi-level match
            if (pattern.StartsWith("*.", StringComparison.Ordinal))
                // Example: ".s3.amazonaws.com"
                return host.EndsWith(pattern.Substring(1), StringComparison.OrdinalIgnoreCase);

            return false;
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