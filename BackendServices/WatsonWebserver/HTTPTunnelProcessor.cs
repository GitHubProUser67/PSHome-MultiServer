using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CastleLibrary.FixedSsl;
using CustomLogger;
using MultiServerLibrary.Extension;
using MultiServerLibrary.Extension.NET;
using MultiServerLibrary.HTTP;
using WatsonWebserver.Core;

namespace WatsonWebserver
{
    public partial class HTTPTunnelProcessor
    {
        private const string newLineHttp = "\r\n\r\n";

        public static int StreamBufferSize { get; set; } = 4096;

        public static async Task HandleClient(
            SslProtocols protocols,
            X509Certificate2 certificate,
            FixedTcpClient tcpClient,
            IPEndPoint endpoint,
            bool httpsTunneling,
            ushort targetPort,
            int milisecondsDelay,
            int maxIncomingHeadersSize
        )
        {
            if (tcpClient == null)
                return;

            //                           123456789012345 6 7 8
            // minimum request 16 bytes: GET / HTTP/1.1\r\n\r\n
            const int preReadLen = 18;

            StringBuilder sb = new StringBuilder();

            using (tcpClient)
            {
                TcpClient client = tcpClient.Client;

                tcpClient.ClientStream = await SslSocket
                    .AuthenticateAsServerAsync(protocols, client.Client, certificate, false, true)
                    .ConfigureAwait(false);

                while (client.IsConnected())
                {
                    if (client.Available > 0)
                    {
                        #region Retrieve-Headers

                        ReadResult readResult = await tcpClient
                            .ReadAsync(milisecondsDelay, preReadLen, StreamBufferSize)
                            .ConfigureAwait(false);

                        // Check for contiguous streams.
                        if (
                            readResult.Status != ReadResultStatus.Success
                            || readResult.BytesRead != preReadLen
                            || readResult.Data == null
                            || readResult.Data.Length != preReadLen
                        )
                        {
                            LoggerAccessor.LogWarn(
                                $"[HTTPTunnelProcessor] - Pre-read test failed,"
                                    + $" shutting down connection id:{tcpClient.GetHashCode()}. (Actual bytes red:{readResult.BytesRead} Expected:{preReadLen})"
                            );
                            return;
                        }
                        else
                        {
                            string httpHeader = Encoding.ASCII.GetString(readResult.Data);

                            if (
                                !string.IsNullOrEmpty(httpHeader)
                                && HttpMethodRegex().IsMatch(httpHeader)
                            )
                            {
                                sb.Clear();

                                sb.Append(httpHeader);

                                bool retrievingHeaders = true;
                                while (retrievingHeaders)
                                {
                                    if (sb.ToString().EndsWith(newLineHttp))
                                        retrievingHeaders = false;
                                    else
                                    {
                                        if (sb.Length >= maxIncomingHeadersSize)
                                        {
                                            LoggerAccessor.LogWarn(
                                                $"[HTTPTunnelProcessor] - Request Header Length exceeded the incomingHeaderSize limit,"
                                                    + $" shutting down connection id:{tcpClient.GetHashCode()}. (Actual size:{sb.Length} Expected:{maxIncomingHeadersSize})"
                                            );
                                            return;
                                        }

                                        ReadResult addlReadResult = await tcpClient
                                            .ReadAsync(milisecondsDelay, 1, StreamBufferSize)
                                            .ConfigureAwait(false); // TODO, optimize this hugely demanding loop.

                                        if (addlReadResult.Status == ReadResultStatus.Success)
                                            sb.Append(
                                                Encoding.ASCII.GetString(addlReadResult.Data)
                                            );
                                        else
                                        {
                                            LoggerAccessor.LogWarn(
                                                $"[HTTPTunnelProcessor] - Add 1 failure, shutting down connection id:{tcpClient.GetHashCode()}."
                                            );
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        bool KeepAlive = false;
                        int ContentLength = 0;
                        string fullUrl = null,
                            ContentType = null,
                            ProtocolVersion = null;
                        HttpMethod Method = HttpMethod.UNKNOWN;

                        #region Convert-to-String-List-And-Dic

                        Dictionary<string, string> headers = [];
                        string[] headersArray = sb.ToString()
                            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

                        #endregion

                        #region Process-Each-Line

                        for (int i = 0; i < headersArray.Length; i++)
                        {
                            if (i == 0)
                            {
                                #region First-Line

                                string[] requestLine = headersArray[i].Trim().Trim('\0').Split(' ');
                                if (requestLine.Length < 3)
                                {
                                    LoggerAccessor.LogWarn(
                                        $"[HTTPTunnelProcessor] - Request line does not contain at least three parts (method, raw URL, protocol/version), shutting down connection id:{tcpClient.GetHashCode()}."
                                    );
                                    return;
                                }

                                string tempUrl = requestLine[1];
                                string tempPath;
                                string host = null;

                                if (Uri.TryCreate(tempUrl, UriKind.Absolute, out Uri absoluteUri))
                                {
                                    // Absolute URL → extract everything from it
                                    tempPath = absoluteUri.PathAndQuery;
                                    host = absoluteUri.Host;
                                }
                                else
                                {
                                    // Relative URL → use configured host/port
                                    tempPath = tempUrl;

                                    IPEndPoint localEndPoint = null;
                                    try
                                    {
                                        localEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                                    }
                                    catch { }

                                    try
                                    {
                                        host = localEndPoint?.Address.ToString();
                                    }
                                    catch
                                    {
                                        // Not Important.
                                    }

                                    if (host == null)
                                        host = IPAddress.Loopback.ToString();
                                }

                                fullUrl =
                                    $"{(httpsTunneling ? "https" : "http")}://{host}:{targetPort}{tempPath}";
                                Method = Enum.Parse<HttpMethod>(requestLine[0], true);

                                ProtocolVersion = requestLine[2];

                                #endregion
                            }
                            else
                            {
                                #region Subsequent-Line

                                int colonIndex = headersArray[i].IndexOf(':');
                                if (colonIndex != -1)
                                {
                                    string key = headersArray[i][..colonIndex].Trim();
                                    string val = headersArray[i][(colonIndex + 1)..].Trim();

                                    if (string.IsNullOrEmpty(key))
                                        continue;

                                    string keyEval = key.ToLower();

                                    if (keyEval.Equals("content-length"))
                                        ContentLength = Convert.ToInt32(val);
                                    else if (keyEval.Equals("content-type"))
                                        ContentType = val;
                                    else if (keyEval.Equals("keep-alive"))
                                        KeepAlive = Convert.ToBoolean(val);

                                    headers.Add(key, val);
                                }

                                #endregion
                            }
                        }

                        #endregion

                        // Perform basic input validation.
                        if (
                            !string.IsNullOrEmpty(fullUrl)
                            && !string.IsNullOrEmpty(ProtocolVersion)
                            && ProtocolVersion.StartsWith("HTTP/")
                        )
                        {
                            (
                                HttpStatusCode? statusCode,
                                byte[] data,
                                Dictionary<string, string> headers
                            ) result;

                            switch (Method)
                            {
                                case HttpMethod.CONNECT:
                                case HttpMethod.OPTIONS:
                                case HttpMethod.HEAD:
                                case HttpMethod.GET:
                                {
                                    result = HTTPProcessor.RequestURL(
                                        fullUrl,
                                        new System.Net.Http.HttpMethod(Method.ToString()),
                                        headers,
                                        null,
                                        null,
                                        true,
                                        KeepAlive
                                    );

                                    break;
                                }
                                case HttpMethod.PUT:
                                case HttpMethod.POST:
                                case HttpMethod.PATCH:
                                case HttpMethod.DELETE:
                                {
                                    const int requestBufferSize = 16 * 1024;

                                    readResult = await tcpClient
                                        .ReadAsync(
                                            milisecondsDelay,
                                            ContentLength,
                                            requestBufferSize
                                        )
                                        .ConfigureAwait(false);
                                    if (readResult.Status != ReadResultStatus.Success)
                                    {
                                        LoggerAccessor.LogWarn(
                                            $"[HTTPTunnelProcessor] - requestBuffer data assigner failed,"
                                                + $" shutting down connection id:{tcpClient.GetHashCode()}."
                                        );
                                        return;
                                    }

                                    result = HTTPProcessor.RequestURL(
                                        fullUrl,
                                        new System.Net.Http.HttpMethod(Method.ToString()),
                                        headers,
                                        readResult.Data,
                                        ContentType ?? "application/octet-stream",
                                        true,
                                        KeepAlive
                                    );

                                    break;
                                }
                                default:
                                {
                                    LoggerAccessor.LogWarn(
                                        $"[HTTPTunnelProcessor] - Unsupported HttpMethod:{Method}, shutting down connection id:{tcpClient.GetHashCode()}."
                                    );
                                    return;
                                }
                            }

                            if (!result.statusCode.HasValue)
                            {
                                LoggerAccessor.LogWarn(
                                    $"[HTTPTunnelProcessor] - Failed to get statusCode from the response, shutting down connection id:{tcpClient.GetHashCode()}."
                                );
                                return;
                            }

                            HttpStatusCode statusCode = result.statusCode.Value;
                            headers = result.headers;

                            await WriteLineToStreamAsync(
                                    milisecondsDelay,
                                    tcpClient,
                                    ToHeader(sb, ProtocolVersion, statusCode, headers)
                                )
                                .ConfigureAwait(false);

                            using (MemoryStream ms = new MemoryStream(result.data))
                            using (
                                HttpResponseContentStream ctwire = new(
                                    milisecondsDelay,
                                    tcpClient,
                                    headers.ContainsKey("Transfer-Encoding")
                                        && headers["Transfer-Encoding"].Contains("chunked")
                                )
                            )
                            {
                                long bytesLeft = ms.Length;

                                StreamUtils.CopyStream(
                                    ms,
                                    ctwire,
                                    bytesLeft > 8000000 && StreamBufferSize < 500000
                                        ? 500000
                                        : StreamBufferSize,
                                    bytesLeft,
                                    true
                                );

                                await ctwire.WriteTerminatorAsync().ConfigureAwait(false);
                            }

                            if ((int)statusCode < 400)
                                LoggerAccessor.LogInfo(
                                    $"[HTTPTunnelProcessor] - {endpoint.Address}:{endpoint.Port} -> tunnel at fullurl: {fullUrl} -> {(int)statusCode}"
                                );
                            else
                            {
                                switch (statusCode)
                                {
                                    case HttpStatusCode.NotFound:
                                    case HttpStatusCode.NotImplemented:
                                    case HttpStatusCode.RequestedRangeNotSatisfiable:
                                        LoggerAccessor.LogWarn(
                                            $"[HTTPTunnelProcessor] - {endpoint.Address}:{endpoint.Port} -> tunnel at fullurl: {fullUrl} -> {(int)statusCode}"
                                        );
                                        break;

                                    default:
                                        LoggerAccessor.LogError(
                                            $"[HTTPTunnelProcessor] - {endpoint.Address}:{endpoint.Port} -> tunnel at fullurl: {fullUrl} -> {(int)statusCode}"
                                        );
                                        break;
                                }
                            }
                        }
                        else
                        {
                            LoggerAccessor.LogWarn(
                                $"[HTTPTunnelProcessor] - Failed to construct the request, shutting down connection id:{tcpClient.GetHashCode()}."
                            );
                            return;
                        }

                        #endregion
                    }
                }
            }
        }

        private static string ToHeader(
            StringBuilder sb,
            string ProtocolVersion,
            HttpStatusCode statusCode,
            Dictionary<string, string> headers
        )
        {
            sb.Clear();

            sb.Append(
                string.Format(
                    "{0} {1} {2}\r\n",
                    ProtocolVersion,
                    (int)statusCode,
                    statusCode.ToString()
                )
            );
            sb.Append(headers.ToHttpHeaders());
            sb.Append(newLineHttp);

            return sb.ToString();
        }

        private static Task<WriteResult> WriteLineToStreamAsync(
            int milisecondsDelay,
            FixedTcpClient tcpClient,
            string text
        )
        {
            return tcpClient.SendAsync(
                milisecondsDelay,
                Encoding.UTF8.GetBytes(text),
                StreamBufferSize
            );
        }

        [System.Text.RegularExpressions.GeneratedRegex(@"^[A-Z]+\s")]
        private static partial System.Text.RegularExpressions.Regex HttpMethodRegex();
    }
}
