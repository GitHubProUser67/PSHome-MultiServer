using WatsonWebserver.Core;
using System.Net;
using CustomLogger;
using ApacheNet.PluginManager;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System;

namespace SonyCdnReroute
{
    public class HTTPPluginReceiver : HTTPPlugin
    {
        private const string internalCdnUrl = "https://PUT_YOUR_CLOUDFLARE_CDN_HOST_HERE";

        Task HTTPPlugin.HTTPStartPlugin(string param, ushort port)
        {
            return Task.CompletedTask;
        }

        // Reuse a single HttpClient instance with a timeout to prevent hanging connections
        private static readonly HttpClient client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30) // Prevent long hangs
        };

        // Static list of valid hosts for faster lookup
        private static readonly HashSet<string> ValidHosts = new HashSet<string>
        {
            "scee-home.playstation.net",
            "scea-home.playstation.net",
            "scej-home.playstation.net",
            "sceasia-home.playstation.net",
            "www.outso-srv1.com",
            "www.capcom.co.jp",
        };

        // Async alternative (recommended for better performance and scalability)
        
        public async Task<object?> ProcessPluginMessageAsync(object obj)
        {
            if (obj is HttpContextBase ctx)
            {
                HttpRequestBase request = ctx.Request;
                HttpResponseBase response = ctx.Response;

                bool sent = false;

                try
                {
                    string host = request.RetrieveHeaderValue("Host");
                    if (ValidHosts.Contains(host))
                    {
                        if (!string.IsNullOrEmpty(request.Url.RawWithQuery) && request.Method == WatsonWebserver.Core.HttpMethod.GET)
                        {
                            const string rangeHeaderConst = "Range";
                            string targetUrl = internalCdnUrl + request.Url.RawWithQuery;

                            using (HttpRequestMessage httpRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, targetUrl))
                            {
                                string rangeHeader = request.RetrieveHeaderValue(rangeHeaderConst);
                                if (!string.IsNullOrEmpty(rangeHeader))
                                    httpRequest.Headers.Add(rangeHeaderConst, rangeHeader);
#if DEBUG
                                LoggerAccessor.LogInfo($"[SonyCdnReroute] - Sending internal CDN redirect to {targetUrl}");
#endif
                                using (HttpResponseMessage cdnResponse = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                                {
                                    response.StatusCode = (int)cdnResponse.StatusCode;

                                    if (cdnResponse.IsSuccessStatusCode || cdnResponse.StatusCode == HttpStatusCode.PartialContent)
                                    {
                                        response.ContentType = cdnResponse.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

                                        if (cdnResponse.StatusCode == HttpStatusCode.PartialContent)
                                        {
                                            var contentRange = cdnResponse.Content.Headers.ContentRange;
                                            if (contentRange != null)
                                            {
                                                response.Headers.Add("Content-Range", $"bytes {contentRange.From}-{contentRange.To}/{contentRange.Length}");
                                                response.Headers.Add("Accept-Ranges", "bytes");
                                            }
                                        }

                                        if (cdnResponse.Content.Headers.ContentLength.HasValue)
                                            response.Headers.Add("Content-Length", cdnResponse.Content.Headers.ContentLength.Value.ToString());

                                        using (Stream contentStream = await cdnResponse.Content.ReadAsStreamAsync().ConfigureAwait(false))
                                        {
                                            const int bufferSize = 512 * 1024; // 512KB buffer
                                            byte[] buffer = new byte[bufferSize];
                                            int bytesRead;
                                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
                                            {
                                                sent = await response.Send(buffer).ConfigureAwait(false);
                                                if (!sent)
                                                    break;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        string errorMessage = cdnResponse.StatusCode == HttpStatusCode.NotFound
                                            ? "Failed to find resource!"
                                            : "Unhandled internal error";
                                        sent = await response.Send(Encoding.ASCII.GetBytes(errorMessage));
                                    }
                                }
                            }
                        }
                        else
                        {
                            response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                            sent = await response.Send(Encoding.ASCII.GetBytes("Only GET requests are supported."));
                        }

                    }
                }
                catch (HttpRequestException ex)
                {
                    response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    sent = await response.Send(Encoding.ASCII.GetBytes("Internal CDN error."));
                    LoggerAccessor.LogError($"[SonyCdnReroute] - HTTP request failed: {ex.Message}");
                }
                catch (Exception ex)
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    sent = await response.Send(Encoding.ASCII.GetBytes("Internal Server error."));
                    LoggerAccessor.LogError($"[SonyCdnReroute] - Unexpected error: {ex.Message}");
                }

                return sent;
            }

            return null;
        }

        object HTTPPlugin.ProcessPluginMessage(object request)
        {
            return ProcessPluginMessageAsync(request);
        }
    }
}