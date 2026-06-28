using System.Collections.Immutable;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using CastleLibrary.FixedSsl.Security.Ssl;
using EndianTools;

namespace CastleLibrary.FixedSsl
{
    public static class SslSocket
    {
        static SslSocket()
        {
#pragma warning disable
            // Enables wildcards certificate support in WebClient.
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;

            // TLS1.3 is only compatible with Windows 10 and Windows server 2019, for now I simply allow TLS1.2 to maintain compatibility, enable yourself if there is a need for 1.3 .
            ServicePointManager.SecurityProtocol |=
                SecurityProtocolType.Tls12 /*| SecurityProtocolType.Tls13*/
            ;
#pragma warning restore
        }

        // Some domains are not valid anymore, but we need them, and know they aren't trapped websites...
        private static readonly string[] _invalidCNBypassList = ["s3.amazonaws.com"];

        private static readonly int SSLv3 = Org.BouncyCastle.Tls.ProtocolVersion.SSLv3.FullVersion; // SSL 3.0
        private static readonly int TLSv1 = Org.BouncyCastle.Tls.ProtocolVersion.TLSv10.FullVersion; // TLS 1.0
        private static readonly int TLSv11 = Org.BouncyCastle
            .Tls
            .ProtocolVersion
            .TLSv11
            .FullVersion; // TLS 1.1
        private static readonly int TLSv12 = Org.BouncyCastle
            .Tls
            .ProtocolVersion
            .TLSv12
            .FullVersion; // TLS 1.2

        private static readonly SecureProtocol legacyProtocols =
            SecureProtocol.Ssl3 | SecureProtocol.Tls1;

        private static readonly ImmutableHashSet<int> SupportedBCCipherSet =
            GetTheadSafeSupportedBCCipherSet();

        // Not thread safe, only use as soon as the program starts before any SSL/TLS operations.
        public static bool BypassRemoteCertificateChecks { get; set; } = false;

        public static List<string> ClientCertificateCNBypassList = []; // Add server CN in which we don't want to validate client certificates.

        private static ImmutableHashSet<int> GetTheadSafeSupportedBCCipherSet()
        {
            return [.. ProtoSSL.GetCipherSuites(Ssl3TlsServer.SupportedProtocols)];
        }

        public static async Task<Stream> AuthenticateAsServerAsync(
            SslProtocols protocols,
            Socket socket,
            X509Certificate2 certificate,
            bool forceSsl,
            bool ownSocket
        )
        {
            // no certificate, no ssl
            if (certificate == null)
                return new NetworkStream(socket, ownSocket);

            // content type - 1 byte
            // version - 2 bytes
            // length - 2 bytes

            // total 5 bytes

            var header = new byte[TlsParser.TLS_HEADER_LEN];
            var received = await socket
                .ReceiveAsync(header, SocketFlags.Peek)
                .ConfigureAwait(false);
            if (received != TlsParser.TLS_HEADER_LEN)
            {
#if DEBUG
                CustomLogger.LoggerAccessor.LogError("[SslSocket] - Invalid header peek.");
#endif
                return null;
            }

            var ssl = header[0] == 0x16; // content type needs to be handshake (0x16)
            var sslV2 = (header[0] & 0x80) != 0 || header[0] == 0x80; // SSLv2 Client Hello indicator

            if (!ssl && !sslV2)
            {
                if (forceSsl)
                {
#if DEBUG
                    CustomLogger.LoggerAccessor.LogError("[SslSocket] - Invalid header data.");
#endif
                    return null;
                }
                return new NetworkStream(socket, ownSocket);
            }

            var totalLength = 0;
            byte[] clientHello = null;

            if (ssl)
            {
                received = 0;

                int r;

                totalLength =
                    TlsParser.TLS_HEADER_LEN
                    + EndianAwareConverter.ToUInt16(header, Endianness.BigEndian, 3);

                clientHello = new byte[totalLength];

                while (received < totalLength)
                {
                    r = socket.Receive(
                        clientHello,
                        received,
                        totalLength - received,
                        SocketFlags.Peek
                    );
                    if (r == 0)
                        break;
                    received += r;
                }

                if (received < totalLength)
                    return null;

                // handshake type needs to be client hello (0x01)
                if (clientHello[5] != 0x01)
                {
                    if (forceSsl)
                    {
#if DEBUG
                        CustomLogger.LoggerAccessor.LogError(
                            "[SslSocket] - Invalid clientHello data."
                        );
#endif
                        return null;
                    }
                    return new NetworkStream(socket, ownSocket);
                }
            }
            else if (sslV2)
            {
                received = 0;

                int r;

                // SSLv2 header: first 2 bytes = 15-bit length
                totalLength = (((header[0] & 0x7F) << 8) | header[1]) + 2; // SSLv2 header length

                clientHello = new byte[totalLength];

                while (received < totalLength)
                {
                    r = socket.Receive(
                        clientHello,
                        received,
                        totalLength - received,
                        SocketFlags.Peek
                    );
                    if (r == 0)
                        break;
                    received += r;
                }

                if (received < totalLength)
                {
#if DEBUG
                    CustomLogger.LoggerAccessor.LogError(
                        $"[SslSocket] - Socket error while picking clientHello data (Excpected:{totalLength} Received:{received})."
                    );
#endif
                    return null;
                }

                // SSLv2 Client Hello validation
                if (clientHello[2] != 0x01) // Message type must be Client Hello
                {
                    if (forceSsl)
                    {
#if DEBUG
                        CustomLogger.LoggerAccessor.LogError(
                            "[SslSocket] - Invalid clientHello data."
                        );
#endif
                        return null;
                    }
                    return new NetworkStream(socket, ownSocket);
                }
            }
            else
            {
                if (forceSsl)
                {
#if DEBUG
                    CustomLogger.LoggerAccessor.LogError("[SslSocket] - Invalid header data.");
#endif
                    return null;
                }
                return new NetworkStream(socket, ownSocket);
            }

            var parseResult = TlsParser.ParseTlsHeader(
                clientHello,
                out var hostname,
                out var isSslV2,
                out var maxSslVersion,
                out var versions,
                out var cipherSuites
            );
#if DEBUG
            CustomLogger.LoggerAccessor.LogInfo(
                $"[SslSocket] - ClientHello (status:{parseResult}) params: Hostname:{hostname} IsSSLV2:{isSslV2} MaxSSLVersion:{maxSslVersion} Versions:{(versions.Count > 0 ? string.Join(", ", versions.Select(v => $"0x{v:X4}")) : "none")} CipherSuites:{(cipherSuites.Count > 0 ? string.Join(", ", cipherSuites.Select(cs => $"0x{cs:X4}")) : "none")}"
            );
#endif
            var allowedProtocols = protocols.GetEnabledProtocols();
#pragma warning disable            // Microsoft doesn't like our FESL exploit, so we fallback to a older crypto supported by Mentalis or BC if that's the case.
            if (
                (
                    allowedProtocols.Contains(SslProtocols.Ssl3)
                    || allowedProtocols.Contains(SslProtocols.Tls)
                    || allowedProtocols.Contains(SslProtocols.Tls11)
                    || allowedProtocols.Contains(SslProtocols.Tls12)
                )
                && (
                    maxSslVersion == SSLv3
                    || maxSslVersion == TLSv1
                    || maxSslVersion == TLSv11
                    || maxSslVersion == TLSv12
                    || (
                        !certificate.Verify()
                        && versions.Any(v => v == SSLv3 || v == TLSv1 || v == TLSv11 || v == TLSv12)
                    )
                )
            )
            {
                Stream managedSsl = await GetBouncyStreamAsync(
                        isSslV2,
                        cipherSuites,
                        certificate,
                        socket,
                        ownSocket
                    )
                    .ConfigureAwait(false);

                if (
                    managedSsl == null
                    && (
                        maxSslVersion != TLSv12
                        || versions.Contains(SSLv3)
                        || versions.Contains(TLSv1)
                        || versions.Contains(TLSv11)
                    )
                ) // Downgrading is fine on these old protocols.
                    managedSsl = await GetMentalisStreamAsync(socket, certificate, ownSocket)
                        .ConfigureAwait(false);

                if (managedSsl != null)
                    return managedSsl;
            }
#pragma warning restore

            var sslStream = new SslStream(new NetworkStream(socket, ownSocket), false);

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

            var header = new byte[TlsParser.TLS_HEADER_LEN];
            var received = socket.Receive(header, SocketFlags.Peek);
            if (received != TlsParser.TLS_HEADER_LEN)
            {
#if DEBUG
                CustomLogger.LoggerAccessor.LogError("[SslSocket] - Invalid header peek.");
#endif
                return null;
            }

            var ssl = header[0] == 0x16; // content type needs to be handshake (0x16)
            var sslV2 = (header[0] & 0x80) != 0 || header[0] == 0x80; // SSLv2 Client Hello indicator

            if (!ssl && !sslV2)
            {
                if (forceSsl)
                {
#if DEBUG
                    CustomLogger.LoggerAccessor.LogError("[SslSocket] - Invalid header data.");
#endif
                    return null;
                }
                return new NetworkStream(socket, ownSocket);
            }

            var totalLength = 0;
            byte[] clientHello = null;

            if (ssl)
            {
                received = 0;

                int r;

                totalLength =
                    TlsParser.TLS_HEADER_LEN
                    + EndianAwareConverter.ToUInt16(header, Endianness.BigEndian, 3);
                ;

                clientHello = new byte[totalLength];

                while (received < totalLength)
                {
                    r = socket.Receive(
                        clientHello,
                        received,
                        totalLength - received,
                        SocketFlags.Peek
                    );
                    if (r == 0)
                        break;
                    received += r;
                }

                if (received < totalLength)
                    return null;

                // handshake type needs to be client hello (0x01)
                if (clientHello[5] != 0x01)
                {
                    if (forceSsl)
                    {
#if DEBUG
                        CustomLogger.LoggerAccessor.LogError(
                            "[SslSocket] - Invalid clientHello data."
                        );
#endif
                        return null;
                    }
                    return new NetworkStream(socket, ownSocket);
                }
            }
            else if (sslV2)
            {
                received = 0;

                int r;

                // SSLv2 header: first 2 bytes = 15-bit length
                totalLength = (((header[0] & 0x7F) << 8) | header[1]) + 2; // SSLv2 header length

                clientHello = new byte[totalLength];

                while (received < totalLength)
                {
                    r = socket.Receive(
                        clientHello,
                        received,
                        totalLength - received,
                        SocketFlags.Peek
                    );
                    if (r == 0)
                        break;
                    received += r;
                }

                if (received < totalLength)
                {
#if DEBUG
                    CustomLogger.LoggerAccessor.LogError(
                        $"[SslSocket] - Socket error while picking clientHello data (Excpected:{totalLength} Received:{received})."
                    );
#endif
                    return null;
                }

                // SSLv2 Client Hello validation
                if (clientHello[2] != 0x01) // Message type must be Client Hello
                {
                    if (forceSsl)
                    {
#if DEBUG
                        CustomLogger.LoggerAccessor.LogError(
                            "[SslSocket] - Invalid clientHello data."
                        );
#endif
                        return null;
                    }
                    return new NetworkStream(socket, ownSocket);
                }
            }
            else
            {
                if (forceSsl)
                {
#if DEBUG
                    CustomLogger.LoggerAccessor.LogError("[SslSocket] - Invalid header data.");
#endif
                    return null;
                }
                return new NetworkStream(socket, ownSocket);
            }

            var parseResult = TlsParser.ParseTlsHeader(
                clientHello,
                out var hostname,
                out var isSslV2,
                out var maxSslVersion,
                out var versions,
                out var cipherSuites
            );
#if DEBUG
            CustomLogger.LoggerAccessor.LogInfo(
                $"[SslSocket] - ClientHello (status:{parseResult}) params: Hostname:{hostname} IsSSLV2:{isSslV2} MaxSSLVersion:{maxSslVersion} Versions:{(versions.Count > 0 ? string.Join(", ", versions.Select(v => $"0x{v:X4}")) : "none")} CipherSuites:{(cipherSuites.Count > 0 ? string.Join(", ", cipherSuites.Select(cs => $"0x{cs:X4}")) : "none")}"
            );
#endif
            var certificate = (X509Certificate2)
                authOptions.ServerCertificateSelectionCallback?.Invoke(socket, hostname);
            if (certificate == null)
            {
                if (forceSsl)
                {
#if DEBUG
                    CustomLogger.LoggerAccessor.LogError(
                        "[SslSocket] - Invalid certificate from callback."
                    );
#endif
                    return null;
                }
                return new NetworkStream(socket, ownSocket);
            }

            var allowedProtocols = authOptions.EnabledSslProtocols.GetEnabledProtocols();
#pragma warning disable
            // Microsoft doesn't like our FESL exploit, so we fallback to a older crypto supported by Mentalis or BC if that's the case.
            if (
                (
                    allowedProtocols.Contains(SslProtocols.Ssl3)
                    || allowedProtocols.Contains(SslProtocols.Tls)
                    || allowedProtocols.Contains(SslProtocols.Tls11)
                    || allowedProtocols.Contains(SslProtocols.Tls12)
                )
                && (
                    maxSslVersion == SSLv3
                    || maxSslVersion == TLSv1
                    || maxSslVersion == TLSv11
                    || maxSslVersion == TLSv12
                    || (
                        !certificate.Verify()
                        && versions.Any(v => v == SSLv3 || v == TLSv1 || v == TLSv11 || v == TLSv12)
                    )
                )
            )
            {
                Stream managedSsl = GetBouncyStreamAsync(
                    isSslV2,
                    cipherSuites,
                    certificate,
                    socket,
                    ownSocket
                ).Result;

                if (
                    managedSsl == null
                    && (
                        maxSslVersion != TLSv12
                        || versions.Contains(SSLv3)
                        || versions.Contains(TLSv1)
                        || versions.Contains(TLSv11)
                    )
                ) // Downgrading is fine on these old protocols.
                    managedSsl = GetMentalisStreamAsync(socket, certificate, ownSocket).Result;

                if (managedSsl != null)
                    return managedSsl;
            }
#pragma warning restore

            int[] clientCertErr = null;
            X509Certificate2 clientCert = null;
            var bypassClientCertValidation = ClientCertificateCNBypassList.Contains(hostname);

            if (
                bypassClientCertValidation
                || authOptions.RemoteCertificateValidationCallback == null
            )
            {
                authOptions.RemoteCertificateValidationCallback = (t, c, ch, e) =>
                {
                    if (c == null)
                        return true;

                    var c2 = c as X509Certificate2;
                    c2 ??= new X509Certificate2(c.GetRawCertData());

                    clientCert = c2;
                    clientCertErr = new int[] { (int)e };
                    return true;
                };
            }

            var sslStream = new SslStream(new NetworkStream(socket, ownSocket), false);

            // Shortcut if status-code is at least -3 or upper
            if (parseResult > -4)
                authOptions.ServerCertificateSelectionCallback = (sender, host) => certificate;

            // Avoids the client cert popup if we don't need it.
            if (authOptions.ClientCertificateRequired && bypassClientCertValidation)
                authOptions.ClientCertificateRequired = false;

            clientCertificate = clientCert;
            clientCertificateErrors = clientCertErr;

            sslStream.AuthenticateAsServer(authOptions);
            return sslStream;
        }

        private static Task<Stream?> GetMentalisStreamAsync(
            Socket socket,
            X509Certificate2 certificate,
            bool ownSocket
        )
        {
            TaskCompletionSource<Stream?> tcs = new(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            var timeoutCts = new CancellationTokenSource(15000); // 15 seconds timeout

            timeoutCts.Token.Register(() =>
            {
                tcs.TrySetResult(null);
            });

            _ = Task.Run(() =>
            {
                try
                {
                    var secureStream = new SecureNetworkStream(
                        new SecureSocket(
                            socket,
                            new SecurityOptions(
                                legacyProtocols,
                                new Security.Certificates.Certificate(certificate),
                                ConnectionEnd.Server,
                                CredentialVerification.Auto,
                                null,
                                null,
                                SecurityFlags.Default,
                                SslAlgorithms.SECURE_CIPHERS,
                                null
                            )
                        ),
                        ownSocket
                    );

                    tcs.TrySetResult(secureStream);
                }
                catch
                {
                    tcs.TrySetResult(null);
                }
                finally
                {
                    timeoutCts.Dispose();
                }
            });

            return tcs.Task;
        }

        private static Task<Stream?> GetBouncyStreamAsync(
            bool isSslV2,
            List<int> cipherSuites,
            X509Certificate2 certificate,
            Socket socket,
            bool ownSocket
        )
        {
            TaskCompletionSource<Stream?> tcs = new(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            var timeoutCts = new CancellationTokenSource(15000); // 15 seconds timeout

            timeoutCts.Token.Register(() =>
            {
                tcs.TrySetResult(null);
            });

            _ = Task.Run(() =>
            {
                try
                {
                    // Make sure BC can handle our request.
                    if (!isSslV2 && cipherSuites.Any(c => SupportedBCCipherSet.Contains(c)))
                    {
                        Ssl3TlsServerProtocol serverProtocol = new(
                            certificate,
                            new NetworkStream(socket, ownSocket)
                        );

                        tcs.TrySetResult(serverProtocol.Stream);

                        return;
                    }

                    throw new Exception();
                }
                catch
                {
                    tcs.TrySetResult(null);
                }
                finally
                {
                    timeoutCts.Dispose();
                }
            });

            return tcs.Task;
        }

        public static IAsyncResult BeginAuthenticateAsServer(
            SslProtocols protocols,
            Socket socket,
            X509Certificate2 certificate,
            bool forceSsl,
            bool ownSocket,
            AsyncCallback callback,
            object state
        )
        {
            return AuthenticateAsServerAsync(protocols, socket, certificate, forceSsl, ownSocket)
                .AsApm(callback, state);
        }

        public static IAsyncResult BeginAuthenticateAsServer(
            Socket socket,
            SslServerAuthenticationOptions authOptions,
            bool forceSsl,
            bool ownSocket,
            AsyncCallback callback,
            object state,
            out X509Certificate2 clientCertificate,
            out int[] clientCertificateErrors
        )
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
                    out localCertErrors
                );
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
            SslPolicyErrors errors
        )
        {
            if (BypassRemoteCertificateChecks || errors == SslPolicyErrors.None)
                return true;

            // Extract CN or SAN hostnames
            var certName = cert
                ?.Subject?.Split(',')
                .Select(s => s.Trim())
                .FirstOrDefault(s => s.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
                ?[3..];

            if (string.IsNullOrEmpty(certName))
                return false;

            // Get requested host from request
            string requestHost = null;
            if (sender is HttpWebRequest req)
                requestHost = req.RequestUri.Host;
            else if (
                sender is SslStream sslStream
                && !string.IsNullOrEmpty(sslStream.TargetHostName)
            )
                requestHost = sslStream.TargetHostName;

            if (string.IsNullOrEmpty(requestHost))
                return false;

            // Custom multi-level dot wildcard check
            if (IsDotWildcardMatch(certName, requestHost))
                return true;
            else if (_invalidCNBypassList.Contains(certName))
                return true;

            CustomLogger.LoggerAccessor.LogError(
                "[SslSocket] - ValidateRemoteCertificate: X509Certificate [{0}] Policy Error: '{1}'",
                cert.Subject,
                errors.ToString()
            );

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
        private static IAsyncResult AsApm<T>(
            this Task<T> task,
            AsyncCallback callback,
            object state
        )
        {
            ArgumentNullException.ThrowIfNull(task);

            var tcs = new TaskCompletionSource<T>(state);
            task.ContinueWith(
                t =>
                {
                    if (t.IsFaulted && t.Exception != null && t.Exception.InnerExceptions != null)
                        tcs.TrySetException(t.Exception.InnerExceptions);
                    else if (t.IsCanceled)
                        tcs.TrySetCanceled();
                    else
                        tcs.TrySetResult(t.Result);

                    if (callback != null)
                        callback(tcs.Task);
                },
                TaskScheduler.Default
            );
            return tcs.Task;
        }

        #endregion
    }
}
