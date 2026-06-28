using System.Net;
using System.Text;
using ApacheNet.Models;
using ApacheNet.PluginManager;
using CustomLogger;
using WatsonWebserver.Core;

namespace SonyCdnReroute
{
    public class HTTPPluginReceiver : IHTTPPlugin
    {
        private static readonly bool pluginEnabled = false;

        private const string internalCdnUrl = "https://PUT_YOUR_CLOUDFLARE_CDN_HOST_HERE";

        Task IHTTPPlugin.HTTPStartPlugin(string param)
        {
            return Task.CompletedTask;
        }

        // Reuse a single HttpClient instance with a timeout to prevent hanging connections
        private static readonly HttpClient client = new()
        {
            Timeout = TimeSpan.FromSeconds(30), // Prevent long hangs
        };

        // Static list of valid hosts for faster lookup
        private static readonly HashSet<string> ValidHosts =
        [
            "scee-home.playstation.net",
            "scea-home.playstation.net",
            "scej-home.playstation.net",
            "sceasia-home.playstation.net",
            "www.outso-srv1.com",
            "www.capcom.co.jp",
        ];

        public static async Task<object?> ProcessPluginMessageAsync(object obj)
        {
            if (!pluginEnabled)
                return null;

            if (obj is HttpContextBase ctx)
            {
                var request = ctx.Request;
                var response = ctx.Response;

                var sent = false;

                try
                {
                    var host = request.RetrieveHeaderValue("Host");
                    if (ValidHosts.Contains(host))
                    {
                        if (
                            !string.IsNullOrEmpty(request.Url.RawWithQuery)
                            && request.Method == WatsonWebserver.Core.HttpMethod.GET
                        )
                        {
                            const string rangeHeaderConst = "Range";
                            var targetUrl = internalCdnUrl + request.Url.RawWithQuery;

                            using (
                                var httpRequest = new HttpRequestMessage(
                                    System.Net.Http.HttpMethod.Get,
                                    targetUrl
                                )
                            )
                            {
                                var rangeHeader = request.RetrieveHeaderValue(rangeHeaderConst);
                                if (!string.IsNullOrEmpty(rangeHeader))
                                    httpRequest.Headers.Add(rangeHeaderConst, rangeHeader);
#if DEBUG
                                LoggerAccessor.LogInfo(
                                    $"[SonyCdnReroute] - Sending internal CDN redirect to {targetUrl}"
                                );
#endif
                                using (
                                    var cdnResponse = await client
                                        .SendAsync(
                                            httpRequest,
                                            HttpCompletionOption.ResponseHeadersRead
                                        )
                                        .ConfigureAwait(false)
                                )
                                {
                                    response.StatusCode = (int)cdnResponse.StatusCode;

                                    if (
                                        cdnResponse.IsSuccessStatusCode
                                        || cdnResponse.StatusCode == HttpStatusCode.PartialContent
                                    )
                                    {
                                        response.ContentType =
                                            cdnResponse.Content.Headers.ContentType?.ToString()
                                            ?? "application/octet-stream";

                                        if (cdnResponse.StatusCode == HttpStatusCode.PartialContent)
                                        {
                                            var contentRange = cdnResponse
                                                .Content
                                                .Headers
                                                .ContentRange;
                                            if (contentRange != null)
                                            {
                                                response.Headers.Add(
                                                    "Content-Range",
                                                    $"bytes {contentRange.From}-{contentRange.To}/{contentRange.Length}"
                                                );
                                                response.Headers.Add("Accept-Ranges", "bytes");
                                            }
                                        }

                                        if (cdnResponse.Content.Headers.ContentLength.HasValue)
                                            response.Headers.Add(
                                                "Content-Length",
                                                cdnResponse.Content.Headers.ContentLength.Value.ToString()
                                            );

                                        using (
                                            var contentStream = await cdnResponse
                                                .Content.ReadAsStreamAsync()
                                                .ConfigureAwait(false)
                                        )
                                        {
                                            const int bufferSize = 512 * 1024; // 512KB buffer
                                            var buffer = new byte[bufferSize];
                                            int bytesRead;
                                            while (
                                                (
                                                    bytesRead = await contentStream
                                                        .ReadAsync(buffer)
                                                        .ConfigureAwait(false)
                                                ) > 0
                                            )
                                            {
                                                var chunk = new byte[bytesRead];
                                                Buffer.BlockCopy(buffer, 0, chunk, 0, bytesRead);
                                                sent = await response
                                                    .Send(chunk)
                                                    .ConfigureAwait(false);
                                                if (!sent)
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        sent = await response
                                            .Send(
                                                Encoding.ASCII.GetBytes(
                                                    cdnResponse.StatusCode
                                                    == HttpStatusCode.NotFound
                                                        ? "Failed to find resource!"
                                                        : "Unhandled internal error"
                                                )
                                            )
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                            sent = await response
                                .Send(Encoding.ASCII.GetBytes("Only GET requests are supported."))
                                .ConfigureAwait(false);
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    sent = await response
                        .Send(Encoding.ASCII.GetBytes("Internal CDN error."))
                        .ConfigureAwait(false);
                    LoggerAccessor.LogError(
                        $"[SonyCdnReroute] - HTTP request failed: {ex.Message}"
                    );
                }
                catch (Exception ex)
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    sent = await response
                        .Send(Encoding.ASCII.GetBytes("Internal Server error."))
                        .ConfigureAwait(false);
                    LoggerAccessor.LogError($"[SonyCdnReroute] - Unexpected error: {ex.Message}");
                }

                return sent;
            }

            return null;
        }

        object IHTTPPlugin.ProcessPluginMessage(object request)
        {
            return ProcessPluginMessageAsync(request);
        }

        public List<Route> GetRoutes()
        {
            return new List<Route> { };
        }
    }
}
