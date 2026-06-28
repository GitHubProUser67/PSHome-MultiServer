using System.Net;
using System.Text;
using ApacheNet.Models;
using ApacheNet.PluginManager;
using MultiServerLibrary.Extension.NET;
using MultiServerLibrary.HTTP;
using WebVideoService.WebVideo;

namespace WebVideoService
{
    public class HTTPPluginReceiver : IHTTPPlugin
    {
        Task IHTTPPlugin.HTTPStartPlugin(string param)
        {
            return Task.CompletedTask;
        }

        public static async Task<object?> ProcessPluginMessageAsync(object obj)
        {
            return null;
        }

        object IHTTPPlugin.ProcessPluginMessage(object request)
        {
            return ProcessPluginMessageAsync(request);
        }

        public List<Route> GetRoutes()
        {
            return
            [
                new()
                {
                    Name = "WebVideo Player",
                    UrlRegex = "^/!player/?$",
                    Method = "GET",
                    Hosts = null,
                    Callable = (ctx) =>
                    {
                        ctx.Response.ChunkedTransfer = ApacheContext.AcceptChunked;
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
                            var ServerIP = ctx.ServerIP;
                            if (ServerIP.Length > 15)
                                ServerIP = "[" + ServerIP + "]"; // Format the hostname if it's a IPV6 url format.
                            WebVideoPlayer? WebPlayer = new(
                                ctx.Request.Query.Elements,
                                $"{(ctx.Secure ? "https" : "http")}://{ServerIP}/!webvideo/?"
                            );
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/html";
                            foreach (var HeaderCollection in WebPlayer.HeadersToSet)
                                ctx.Response.Headers.Add(HeaderCollection[0], HeaderCollection[1]);
                            return ctx.Response.ChunkedTransfer
                                ? ctx
                                    .Response.SendChunk(
                                        Encoding.UTF8.GetBytes(WebPlayer.HtmlPage),
                                        true
                                    )
                                    .Result
                                : ctx.Response.Send(WebPlayer.HtmlPage).Result;
                        }
                        ctx.Response.ChunkedTransfer = false;
                        ctx.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                        ctx.Response.Send().Wait();
                        return true;
                    },
                },
                new()
                {
                    Name = "WebVideo Processor",
                    UrlRegex = "^/!webvideo/?$",
                    Method = "GET",
                    Hosts = null,
                    Callable = (ctx) =>
                    {
                        ctx.Response.ChunkedTransfer = ApacheContext.AcceptChunked;
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
                            var QueryDic = HTTPProcessor.GetQueryParameters(ctx.FullUrl);
                            if (
                                QueryDic != null
                                && QueryDic.Count > 0
                                && QueryDic.TryGetValue("url", out var queryUrl)
                                && !string.IsNullOrEmpty(queryUrl)
                            )
                            {
                                var vid = WebVideoConverter.ConvertVideo(
                                    QueryDic,
                                    ApacheNetServerConfiguration.MediaConvertersFolder
                                );
                                if (vid != null && vid.Available)
                                {
                                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                    ctx.Response.ContentType = vid.ContentType;
                                    ctx.Response.Headers.Add(
                                        "Content-Disposition",
                                        "attachment; filename=\"" + vid.FileName + "\""
                                    );
                                    const int buffersize = 16 * 1024;
                                    HugeMemoryStream videoStream = new(vid.VideoStream, buffersize);
                                    if (ctx.Response.ChunkedTransfer)
                                    {
                                        var bytesLeft = videoStream.Length;

                                        if (bytesLeft == 0)
                                            return ctx.Response.SendChunk([], true).Result;
                                        else
                                        {
                                            bool isNotlastChunk;
                                            byte[] buffer;

                                            while (bytesLeft > 0)
                                            {
                                                isNotlastChunk = bytesLeft > buffersize;
                                                buffer = new byte[
                                                    isNotlastChunk ? buffersize : bytesLeft
                                                ];
                                                var n = videoStream.Read(buffer, 0, buffer.Length);

                                                if (isNotlastChunk)
                                                    ctx.Response.SendChunk(buffer, false).Wait();
                                                else
                                                    return ctx
                                                        .Response.SendChunk(buffer, true)
                                                        .Result;

                                                bytesLeft -= n;
                                            }
                                        }
                                    }
                                    else
                                        return ctx
                                            .Response.Send(videoStream.Length, videoStream)
                                            .Result;
                                }
                                else
                                {
                                    var htmlPayloadVideo =
                                        "<p>"
                                        + vid?.ErrorMessage
                                        + "</p>"
                                        + "<p>Make sure that parameters are correct, and both <i>yt-dlp</i> and <i>ffmpeg</i> are properly installed on the server.</p>";
                                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                    ctx.Response.ContentType = "text/html";
                                    return ctx.Response.ChunkedTransfer
                                        ? ctx
                                            .Response.SendChunk(
                                                Encoding.UTF8.GetBytes(htmlPayloadVideo),
                                                true
                                            )
                                            .Result
                                        : ctx.Response.Send(htmlPayloadVideo).Result;
                                }
                            }
                            else
                            {
                                const string webVideoTutorialHtmlPayload =
                                    "<p>MultiServer can help download videos from popular sites in preferred format.</p>"
                                    + "<p>Manual use parameters:"
                                    + "<ul>"
                                    + "<li><b>url</b> - Address of the video (e.g. https://www.youtube.com/watch?v=fPnO26CwqYU or similar)</li>"
                                    + "<li><b>f</b> - Target format of the file (e.g. avi)</li>"
                                    + "<li><b>vcodec</b> - Codec for video (e.g. mpeg4)</li>"
                                    + "<li><b>acodec</b> - Codec for audio (e.g. mp3)</li>"
                                    + "<li><b>content-type</b> - override MIME content type for the file (optional).</li>"
                                    + "<li>Also you can use many <i>yt-dlp"
                                    + "</i> and <i>ffmpeg"
                                    + "</i> options like <b>aspect</b>, <b>b</b>, <b>no-mark-watched</b> and other.</li>"
                                    + "</ul></p>";
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/html";
                                return ctx.Response.ChunkedTransfer
                                    ? ctx
                                        .Response.SendChunk(
                                            Encoding.UTF8.GetBytes(webVideoTutorialHtmlPayload),
                                            true
                                        )
                                        .Result
                                    : ctx.Response.Send(webVideoTutorialHtmlPayload).Result;
                            }
                        }
                        ctx.Response.ChunkedTransfer = false;
                        ctx.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                        ctx.Response.Send().Wait();
                        return true;
                    },
                },
            ];
        }
    }
}
