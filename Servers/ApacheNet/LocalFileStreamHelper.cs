using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ApacheNet.Models;
using MultiServerLibrary.Extension;
using MultiServerLibrary.Extension.NET;
using MultiServerLibrary.HTTP;
using MultiServerLibrary.Upscalers;

namespace ApacheNet
{
    public partial class LocalFileStreamHelper
    {
        public const int FileLockAwaitMs = 500;

        public const long compressionSizeLimit = 800L * 1024 * 1024; // 800MB in bytes

        public static async Task<bool> HandleRequest(
            ApacheContext ctx,
            string encoding,
            string absolutepath,
            string filePath,
            string ContentType,
            string UserAgent,
            bool isVideoOrAudio,
            bool isHtmlCompatible,
            bool noCompressCacheControl
        )
        {
            var ifModifiedSince = ctx.Request.RetrieveHeaderValue("If-Modified-Since");
            var NoneMatch = ctx.Request.RetrieveHeaderValue("If-None-Match");
            var EtagMD5 = HTTPProcessor.ETag(filePath);

            if (!string.IsNullOrEmpty(EtagMD5))
                ctx.Response.Headers.Add("ETag", EtagMD5);

            if (
                !string.IsNullOrEmpty(NoneMatch)
                    ? NoneMatch == EtagMD5
                    : !string.IsNullOrEmpty(ifModifiedSince)
                        && HTTPProcessor.CheckLastWriteTime(filePath, ifModifiedSince)
            )
            {
                ctx.Response.ContentType = "text/plain";
                ctx.StatusCode = HttpStatusCode.NotModified;
                return await ctx.SendImmediate().ConfigureAwait(false);
            }

            Stream? st;
            var compressionSettingEnabled = ApacheNetServerConfiguration.EnableHTTPCompression;
            var extension = Path.GetExtension(filePath);

            if (
                ApacheNetServerConfiguration.EnableImageUpscale
                && (
                    (!string.IsNullOrEmpty(ContentType) && ContentType.StartsWith("image/"))
                    || (
                        !string.IsNullOrEmpty(extension)
                        && extension.Equals(".dds", StringComparison.InvariantCultureIgnoreCase)
                    )
                )
            )
            {
                ctx.Response.ContentType = ContentType;

                try
                {
                    st = ImageOptimizer.OptimizeImage(
                        ApacheNetServerConfiguration.MediaConvertersFolder,
                        Path.Combine(
                            ApacheNetServerConfiguration.MediaConvertersFolder,
                            "ImageMagick"
                        ),
                        filePath,
                        extension,
                        ImageOptimizer.defaultOptimizerParams
                    );

                    if (
                        compressionSettingEnabled
                        && !noCompressCacheControl
                        && !string.IsNullOrEmpty(encoding)
                        && st.Length <= compressionSizeLimit
                    )
                    {
                        if (encoding.Contains("zstd"))
                        {
                            ctx.Response.Headers.Add("Content-Encoding", "zstd");
                            st = HTTPProcessor.ZstdCompressStream(st);
                        }
                        else if (encoding.Contains("br"))
                        {
                            ctx.Response.Headers.Add("Content-Encoding", "br");
                            st = HTTPProcessor.BrotliCompressStream(st);
                        }
                        else if (encoding.Contains("gzip"))
                        {
                            ctx.Response.Headers.Add("Content-Encoding", "gzip");
                            st = HTTPProcessor.GzipCompressStream(st);
                        }
                        else if (encoding.Contains("deflate"))
                        {
                            ctx.Response.Headers.Add("Content-Encoding", "deflate");
                            st = HTTPProcessor.DeflateStream(st);
                        }
                    }
                }
                catch
                {
                    st = null;
                }
            }
            else if (isHtmlCompatible && isVideoOrAudio)
            {
                string htmlContent;
                // Generate an HTML page with the video element
                if (!string.IsNullOrEmpty(UserAgent) && UserAgent.Contains("PLAYSTATION 3"))
                {
                    switch (ctx.Request.RetrieveQueryValue("PS3"))
                    {
                        case "play":
                            ctx.Response.ContentType = ContentType;

                            ctx.Response.Headers.Add("Accept-Ranges", "bytes");

                            if (
                                compressionSettingEnabled
                                && !noCompressCacheControl
                                && !string.IsNullOrEmpty(encoding)
                                && new FileInfo(filePath).Length <= compressionSizeLimit
                            )
                            {
                                if (encoding.Contains("gzip"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                    st = HTTPProcessor.GzipCompressStream(
                                        await FileSystemUtils
                                            .TryOpen(filePath, FileShare.ReadWrite, FileLockAwaitMs)
                                            .ConfigureAwait(false)
                                    );
                                }
                                else if (encoding.Contains("deflate"))
                                {
                                    ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                    st = HTTPProcessor.DeflateStream(
                                        await FileSystemUtils
                                            .TryOpen(filePath, FileShare.ReadWrite, FileLockAwaitMs)
                                            .ConfigureAwait(false)
                                    );
                                }
                                else
                                    st = await FileSystemUtils
                                        .TryOpen(filePath, FileShare.ReadWrite, FileLockAwaitMs)
                                        .ConfigureAwait(false);
                            }
                            else
                                st = await FileSystemUtils
                                    .TryOpen(filePath, FileShare.ReadWrite, FileLockAwaitMs)
                                    .ConfigureAwait(false);

                            goto sendImmediate;
                        default:
#if DEBUG
                            var debug = true;
#else
                            bool debug = false;
#endif
                            // The HDK documentation states that only Flash player 7 is supported on the "in-game" browser mode (silk_npflashplayer.sprx). Normal browser uses Flash Player 9 (silk_npflashplayer9.sprx).
                            var flashPlayer7 = true;
                            if (ctx.Request.HeaderExists("x-ps3-browser"))
                            {
                                var match = MyRegex()
                                    .Match(ctx.Request.RetrieveHeaderValue("x-ps3-browser"));
                                if (match.Success)
                                    flashPlayer7 =
                                        double.Parse(
                                            match.Groups[1].Value,
                                            CultureInfo.InvariantCulture
                                        ) < 2.50;
                            }
                            var isSupported = HTTPProcessor.IsPS3SupportedContentType(ContentType);
                            htmlContent =
                                $@"
                                <!DOCTYPE html>
                                <html>
                                <head>
                                  <title>PlayStation Media Player</title>
                                  <style>
                                    body {{
                                      background-color: #000000;
                                      color: #FFFFFF;
                                      text-align: center;
                                      font-family: Arial, sans-serif;
                                      margin: 0;
                                      padding: 20px;
                                    }}
                                    h1 {{
                                      font-size: 24px;
                                      margin-bottom: 20px;
                                    }}
                                    a.button {{
                                      display: inline-block;
                                      background-color: #0070D1;
                                      color: #FFFFFF;
                                      padding: 14px 28px;
                                      text-decoration: none;
                                      border-radius: 8px;
                                      font-size: 18px;
                                    }}
                                    a.button:hover {{
                                      background-color: #0055A4;
                                    }}
                                    p {{
                                      margin-top: 40px;
                                      font-size: 14px;
                                      color: #AAAAAA;
                                    }}
                                  </style>
                                </head>
                                <script type=""text/javascript"">
                                   {(flashPlayer7 ? @$"function playerReady() {{
                                    {(debug ? "alert(\"DEBUG: Media player loaded.\");" : string.Empty)}
                                  }}" : @$"function printTrace() {{
                                    {(debug ? "alert(\"DEBUG: Media player loaded.\");" : string.Empty)}
                                  }}")}
                                </script>
                                <body>
                                  <h1>Media Player</h1>
                                  <a class='button' href='{absolutepath}?PS3=play' target='_blank'>{(isSupported ? "▶ Download Video" : "Backup Video to external storage")}</a>
                                  <p>Media {(isSupported ? string.Empty : "not ")}compatible with the PlayStation 3 System</p>
                                  {$@"{(IsJWPlayerCompatibleFormat(ContentType) ? $@"<br />
                                    <object
                                        type=""application/x-shockwave-flash""
                                        data=""/jwplayer/player{(flashPlayer7 ? "43" : "53")}.swf""
                                        width=""860""
                                        height=""580"">
                                        <param name=""allowfullscreen"" value=""true"" />
                                        <param name=""flashvars"" value=""controlbar=bottom&file={absolutepath}?PS3=play"" />
                                    </object>" : string.Empty)}"}
                                </body>
                                </html>";
                            break;
                    }
                }
                else // TODO, support more older browsers?
                {
                    htmlContent =
                        @"
                            <!DOCTYPE html>
                            <html>
                            <head>
                              <title>Secure Web Media Player</title>
                              <style>
                                body {
                                  display: flex;
                                  justify-content: center;
                                  align-items: center;
                                  height: 100vh;
                                  margin: 0;
                                  background-color: black;
                                }
                                #video-container {
                                  max-width: 100%;
                                  max-height: 100%;
                                }
                                video {
                                  width: 100%;
                                  height: 100%;
                                  object-fit: contain;
                                }
                              </style>
                            </head>
                            <body>
                              <div id=""video-container"">
                                <video controls>
                                  <source src="""
                        + absolutepath
                        + $@""" type=""{ContentType}"">
                                </video>
                              </div>
                            </body>
                            </html>";
                }

                ctx.Response.ContentType = "text/html; charset=UTF-8";

                var htmlMs = new MemoryStream(Encoding.UTF8.GetBytes(htmlContent));

                if (
                    compressionSettingEnabled
                    && !noCompressCacheControl
                    && !string.IsNullOrEmpty(encoding)
                )
                {
                    if (encoding.Contains("zstd"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "zstd");
                        st = HTTPProcessor.ZstdCompressStream(htmlMs);
                    }
                    else if (encoding.Contains("br"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "br");
                        st = HTTPProcessor.BrotliCompressStream(htmlMs);
                    }
                    else if (encoding.Contains("gzip"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "gzip");
                        st = HTTPProcessor.GzipCompressStream(htmlMs);
                    }
                    else if (encoding.Contains("deflate"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "deflate");
                        st = HTTPProcessor.DeflateStream(htmlMs);
                    }
                    else
                        st = htmlMs;
                }
                else
                    st = htmlMs;
            }
            else
            {
                ctx.Response.ContentType = ContentType;

                if (
                    compressionSettingEnabled
                    && !noCompressCacheControl
                    && !string.IsNullOrEmpty(encoding)
                    && new FileInfo(filePath).Length <= compressionSizeLimit
                )
                {
                    if (encoding.Contains("zstd"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "zstd");
                        st = HTTPProcessor.ZstdCompressStream(
                            await FileSystemUtils
                                .TryOpen(filePath, FileShare.ReadWrite, FileLockAwaitMs)
                                .ConfigureAwait(false)
                        );
                    }
                    else if (encoding.Contains("br"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "br");
                        st = HTTPProcessor.BrotliCompressStream(
                            await FileSystemUtils
                                .TryOpen(filePath, FileShare.ReadWrite, FileLockAwaitMs)
                                .ConfigureAwait(false)
                        );
                    }
                    else if (encoding.Contains("gzip"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "gzip");
                        st = HTTPProcessor.GzipCompressStream(
                            await FileSystemUtils
                                .TryOpen(filePath, FileShare.ReadWrite, FileLockAwaitMs)
                                .ConfigureAwait(false)
                        );
                    }
                    else if (encoding.Contains("deflate"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "deflate");
                        st = HTTPProcessor.DeflateStream(
                            await FileSystemUtils
                                .TryOpen(filePath, FileShare.ReadWrite, FileLockAwaitMs)
                                .ConfigureAwait(false)
                        );
                    }
                    else
                        st = await FileSystemUtils
                            .TryOpen(filePath, FileShare.ReadWrite, FileLockAwaitMs)
                            .ConfigureAwait(false);
                }
                else
                    st = await FileSystemUtils
                        .TryOpen(filePath, FileShare.ReadWrite, FileLockAwaitMs)
                        .ConfigureAwait(false);
            }
            sendImmediate:
            if (st == null)
            {
                ctx.Response.Headers.Clear();
                ctx.StatusCode = HttpStatusCode.InternalServerError;
                ctx.Response.ContentType = "text/plain";
                return await ctx.SendImmediate().ConfigureAwait(false);
            }

            var chunked = ApacheContext.AcceptChunked;

            // Hotfix PSHome videos not being displayed in HTTP using chunck encoding (game bug).
            if (
                !string.IsNullOrEmpty(ctx.Request.Useragent)
                && ctx.Request.Useragent.Contains("PSHome")
                && isVideoOrAudio
            )
                chunked = false;

            ctx.StatusCode = HttpStatusCode.OK;
            ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
            ctx.Response.Headers.Add(
                "Last-Modified",
                File.GetLastWriteTime(filePath).ToString("r")
            );
            ctx.Response.Headers.Add("Expires", DateTime.Now.AddMinutes(30).ToString("r"));

            using (st)
                return await ctx.SendImmediate(st, chunked).ConfigureAwait(false);
        }

        public static async Task<bool> HandlePartialRangeRequest(
            ApacheContext ctx,
            string filePath,
            string ContentType,
            bool noCompressCacheControl,
            string boundary = "multiserver_separator"
        )
        {
            // This method directly communicate with the wire to handle, normally, imposible transfers.
            // If a part of the code sounds weird to you, it's normal... So does curl tests...

            if (
                HTTPProcessor.CheckLastWriteTime(
                    filePath,
                    ctx.Request.RetrieveHeaderValue("If-Unmodified-Since"),
                    true
                )
            )
            {
                ctx.StatusCode = HttpStatusCode.PreconditionFailed;
                return await ctx.SendImmediate().ConfigureAwait(false);
            }
            else
            {
                try
                {
                    var fs = await FileSystemUtils
                        .TryOpen(filePath, FileShare.ReadWrite, FileLockAwaitMs)
                        .ConfigureAwait(false);

                    if (fs != null)
                    {
                        const int rangebuffersize = 32768;

                        var acceptencoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");

                        using (fs)
                        {
                            long startByte = -1;
                            long endByte = -1;
                            var filesize = fs.Length;
                            var HeaderString = ctx
                                .Request.RetrieveHeaderValue("Range")
                                .Replace("bytes=", string.Empty);
                            if (HeaderString.Contains(','))
                            {
                                var multipartSeparator = Encoding.UTF8.GetBytes($"--{boundary}--");
                                var Separator = "\r\n"u8.ToArray();

                                using HugeMemoryStream ms = new();
                                // Split the ranges based on the comma (',') separator
                                foreach (var RangeSelect in HeaderString.Split(','))
                                {
                                    var contentTypeBytes = Encoding.UTF8.GetBytes(
                                        $"Content-Type: {ContentType}"
                                    );
                                    ms.Write(Separator, 0, Separator.Length);
                                    ms.Write(multipartSeparator, 0, multipartSeparator.Length - 2);
                                    ms.Write(Separator, 0, Separator.Length);
                                    ms.Write(contentTypeBytes, 0, contentTypeBytes.Length);
                                    ms.Write(Separator, 0, Separator.Length);
                                    fs.Position = 0;
                                    startByte = -1;
                                    endByte = -1;
                                    var range = RangeSelect.Split('-');
                                    if (range[0].Trim().Length > 0)
                                        _ = long.TryParse(range[0], out startByte);
                                    if (range[1].Trim().Length > 0)
                                        _ = long.TryParse(range[1], out endByte);
                                    if (endByte == -1)
                                        endByte = filesize;
                                    else if (endByte != filesize)
                                        endByte++;
                                    if (startByte == -1)
                                    {
                                        startByte = filesize - endByte;
                                        endByte = filesize;
                                    }
                                    if (endByte > filesize) // Curl test showed this behaviour.
                                        endByte = filesize;
                                    if (startByte >= filesize && endByte == filesize) // Curl test showed this behaviour.
                                    {
                                        byte[] payloadBytes;
                                        const string payload =
                                            "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\r\n"
                                            + "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"\r\n"
                                            + "         \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n"
                                            + "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">\r\n"
                                            + "        <head>\r\n"
                                            + "                <title>416 - Requested Range Not Satisfiable</title>\r\n"
                                            + "        </head>\r\n"
                                            + "        <body>\r\n"
                                            + "                <h1>416 - Requested Range Not Satisfiable</h1>\r\n"
                                            + "        </body>\r\n"
                                            + "</html>";

                                        ms.Flush();
                                        ms.Close();
                                        fs.Flush();
                                        fs.Close();
                                        ctx.Response.Headers.Add(
                                            "Content-Range",
                                            string.Format("bytes */{0}", filesize)
                                        );
                                        ctx.StatusCode =
                                            HttpStatusCode.RequestedRangeNotSatisfiable;
                                        ctx.Response.ContentType = "text/html; charset=UTF-8";
                                        if (
                                            ApacheNetServerConfiguration.EnableHTTPCompression
                                            && !noCompressCacheControl
                                            && !string.IsNullOrEmpty(acceptencoding)
                                        )
                                        {
                                            if (acceptencoding.Contains("zstd"))
                                            {
                                                ctx.Response.Headers.Add(
                                                    "Content-Encoding",
                                                    "zstd"
                                                );
                                                payloadBytes = HTTPProcessor.CompressZstd(
                                                    Encoding.UTF8.GetBytes(payload)
                                                );
                                            }
                                            else if (acceptencoding.Contains("br"))
                                            {
                                                ctx.Response.Headers.Add("Content-Encoding", "br");
                                                payloadBytes = HTTPProcessor.CompressBrotli(
                                                    Encoding.UTF8.GetBytes(payload)
                                                );
                                            }
                                            else if (acceptencoding.Contains("gzip"))
                                            {
                                                ctx.Response.Headers.Add(
                                                    "Content-Encoding",
                                                    "gzip"
                                                );
                                                payloadBytes = HTTPProcessor.CompressGzip(
                                                    Encoding.UTF8.GetBytes(payload)
                                                );
                                            }
                                            else if (acceptencoding.Contains("deflate"))
                                            {
                                                ctx.Response.Headers.Add(
                                                    "Content-Encoding",
                                                    "deflate"
                                                );
                                                payloadBytes = HTTPProcessor.Deflate(
                                                    Encoding.UTF8.GetBytes(payload)
                                                );
                                            }
                                            else
                                                payloadBytes = Encoding.UTF8.GetBytes(payload);
                                        }
                                        else
                                            payloadBytes = Encoding.UTF8.GetBytes(payload);

                                        return await ctx.SendImmediate(
                                                payloadBytes,
                                                ApacheContext.AcceptChunked
                                            )
                                            .ConfigureAwait(false);
                                    }
                                    else if (
                                        (startByte >= endByte)
                                        || startByte < 0
                                        || endByte <= 0
                                    ) // Curl test showed this behaviour.
                                    {
                                        ms.Flush();
                                        ms.Close();
                                        fs.Position = 0;

                                        ctx.Response.Headers.Add(
                                            "Date",
                                            DateTime.Now.ToString("r")
                                        );
                                        ctx.Response.Headers.Add(
                                            "Last-Modified",
                                            File.GetLastWriteTime(filePath).ToString("r")
                                        );
                                        ctx.Response.Headers.Add("Accept-Ranges", "bytes");
                                        ctx.StatusCode = HttpStatusCode.OK;
                                        ctx.Response.ContentType = ContentType;
                                        return await ctx.SendImmediate(
                                                fs,
                                                ApacheContext.AcceptChunked
                                            )
                                            .ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        var bytesRead = 0;
                                        var TotalBytes = endByte - startByte;
                                        long totalBytesCopied = 0;
                                        var buffer = new byte[rangebuffersize];
                                        var contentRangeBytes = Encoding.UTF8.GetBytes(
                                            "Content-Range: "
                                                + string.Format(
                                                    "bytes {0}-{1}/{2}",
                                                    startByte,
                                                    endByte - 1,
                                                    filesize
                                                )
                                        );
                                        fs.Position = startByte;
                                        ms.Write(contentRangeBytes, 0, contentRangeBytes.Length);
                                        ms.Write(Separator, 0, Separator.Length);
                                        ms.Write(Separator, 0, Separator.Length);
                                        while (
                                            totalBytesCopied < TotalBytes
                                            && (
                                                bytesRead = await fs.ReadAsync(
                                                        buffer.AsMemory(0, rangebuffersize)
                                                    )
                                                    .ConfigureAwait(false)
                                            ) > 0
                                        )
                                        {
                                            var bytesToWrite = (int)
                                                Math.Min(TotalBytes - totalBytesCopied, bytesRead);
                                            ms.Write(buffer, 0, bytesToWrite);
                                            totalBytesCopied += bytesToWrite;
                                        }
                                    }
                                }
                                ms.Write(Separator, 0, Separator.Length);
                                ms.Write(multipartSeparator, 0, multipartSeparator.Length);
                                ms.Write(Separator, 0, Separator.Length);
                                ms.Position = 0;
                                ctx.Response.ContentType =
                                    $"multipart/byteranges; boundary={boundary}";
                                ctx.Response.Headers.Add("Accept-Ranges", "bytes");
                                ctx.Response.Headers.Add("Content-Length", ms.Length.ToString());
                                ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                ctx.Response.Headers.Add(
                                    "Last-Modified",
                                    File.GetLastWriteTime(filePath).ToString("r")
                                );
                                ctx.StatusCode = HttpStatusCode.PartialContent;

                                fs.Flush();
                                fs.Close();

                                return await ctx.SendImmediate(ms, ApacheContext.AcceptChunked)
                                    .ConfigureAwait(false);
                            }
                            else
                            {
                                var range = HeaderString.Split('-');
                                if (range[0].Trim().Length > 0)
                                    _ = long.TryParse(range[0], out startByte);
                                if (range[1].Trim().Length > 0)
                                    _ = long.TryParse(range[1], out endByte);
                                if (endByte == -1)
                                    endByte = filesize;
                                else if (endByte != filesize)
                                    endByte++;
                                if (startByte == -1)
                                {
                                    startByte = filesize - endByte;
                                    endByte = filesize;
                                }
                            }
                            if (endByte > filesize) // Curl test showed this behaviour.
                                endByte = filesize;
                            if (startByte >= filesize && endByte == filesize) // Curl test showed this behaviour.
                            {
                                byte[] payloadBytes;
                                const string payload =
                                    "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\r\n"
                                    + "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"\r\n"
                                    + "         \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n"
                                    + "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">\r\n"
                                    + "        <head>\r\n"
                                    + "                <title>416 - Requested Range Not Satisfiable</title>\r\n"
                                    + "        </head>\r\n"
                                    + "        <body>\r\n"
                                    + "                <h1>416 - Requested Range Not Satisfiable</h1>\r\n"
                                    + "        </body>\r\n"
                                    + "</html>";

                                fs.Flush();
                                fs.Close();
                                ctx.Response.Headers.Add(
                                    "Content-Range",
                                    string.Format("bytes */{0}", filesize)
                                );
                                ctx.StatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
                                ctx.Response.ContentType = "text/html; charset=UTF-8";
                                if (
                                    ApacheNetServerConfiguration.EnableHTTPCompression
                                    && !noCompressCacheControl
                                    && !string.IsNullOrEmpty(acceptencoding)
                                )
                                {
                                    if (acceptencoding.Contains("zstd"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                        payloadBytes = HTTPProcessor.CompressZstd(
                                            Encoding.UTF8.GetBytes(payload)
                                        );
                                    }
                                    else if (acceptencoding.Contains("br"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "br");
                                        payloadBytes = HTTPProcessor.CompressBrotli(
                                            Encoding.UTF8.GetBytes(payload)
                                        );
                                    }
                                    else if (acceptencoding.Contains("gzip"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                        payloadBytes = HTTPProcessor.CompressGzip(
                                            Encoding.UTF8.GetBytes(payload)
                                        );
                                    }
                                    else if (acceptencoding.Contains("deflate"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                        payloadBytes = HTTPProcessor.Deflate(
                                            Encoding.UTF8.GetBytes(payload)
                                        );
                                    }
                                    else
                                        payloadBytes = Encoding.UTF8.GetBytes(payload);
                                }
                                else
                                    payloadBytes = Encoding.UTF8.GetBytes(payload);

                                return await ctx.SendImmediate(
                                        payloadBytes,
                                        ApacheContext.AcceptChunked
                                    )
                                    .ConfigureAwait(false);
                            }
                            else if ((startByte >= endByte) || startByte < 0 || endByte <= 0) // Curl test showed this behaviour.
                            {
                                fs.Position = 0;

                                ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                ctx.Response.Headers.Add(
                                    "Last-Modified",
                                    File.GetLastWriteTime(filePath).ToString("r")
                                );
                                ctx.Response.Headers.Add("Accept-Ranges", "bytes");
                                ctx.StatusCode = HttpStatusCode.OK;
                                ctx.Response.ContentType = ContentType;

                                return await ctx.SendImmediate(fs, ApacheContext.AcceptChunked)
                                    .ConfigureAwait(false);
                            }
                            else
                            {
                                ctx.Response.ChunkedTransfer = ApacheContext.AcceptChunked;

                                var TotalBytes = endByte - startByte;
                                fs.Position = startByte;
                                ctx.Response.ContentType = ContentType;
                                ctx.Response.Headers.Add("Accept-Ranges", "bytes");
                                ctx.Response.Headers.Add(
                                    "Content-Range",
                                    string.Format(
                                        "bytes {0}-{1}/{2}",
                                        startByte,
                                        endByte - 1,
                                        filesize
                                    )
                                );
                                ctx.Response.Headers.Add("Content-Length", TotalBytes.ToString());
                                ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                ctx.Response.Headers.Add(
                                    "Last-Modified",
                                    File.GetLastWriteTime(filePath).ToString("r")
                                );
                                ctx.Response.StatusCode = (int)HttpStatusCode.PartialContent;

                                if (ctx.Response.ChunkedTransfer)
                                {
                                    if (TotalBytes == 0)
                                        return await ctx
                                            .Response.SendChunk([], true)
                                            .ConfigureAwait(false);

                                    var bufferSize = ApacheNetServerConfiguration.BufferSize;

                                    bool isNotlastChunk;
                                    byte[] buffer;

                                    while (TotalBytes > 0)
                                    {
                                        isNotlastChunk = TotalBytes > bufferSize;
                                        buffer = new byte[isNotlastChunk ? bufferSize : TotalBytes];
                                        var n = await fs.ReadAsync(buffer).ConfigureAwait(false);

                                        if (isNotlastChunk)
                                            await ctx
                                                .Response.SendChunk(buffer, false)
                                                .ConfigureAwait(false);
                                        else
                                            return await ctx
                                                .Response.SendChunk(buffer, true)
                                                .ConfigureAwait(false);

                                        TotalBytes -= n;
                                    }
                                }
                                else
                                    return await ctx
                                        .Response.Send(TotalBytes, fs)
                                        .ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch
                {
                    // Not Important
                }

                ctx.StatusCode = HttpStatusCode.InternalServerError;
                return await ctx.SendImmediate().ConfigureAwait(false);
            }
        }

        private static bool IsJWPlayerCompatibleFormat(string contentType)
        {
            // Normalize to lowercase for comparison
            contentType = contentType.ToLowerInvariant();

            // List of compatible MIME types for JW Player Flash mode
            foreach (
                var type in new[]
                {
                    "video/x-flv", // FLV video
                    "video/mp4", // MP4 video (H.264 + AAC)
                    "video/mpeg", // Sometimes for .3gp or MPEG-4
                    "audio/mpeg", // MP3 audio
                    "audio/mp3", // MP3 audio (sometimes used)
                    "audio/aac", // AAC audio
                    "audio/x-aac", // AAC audio
                    "video/3gpp", // 3GP video (if H.264 + AAC)
                    "video/quicktime", // MOV (H.264 + AAC)
                    "video/x-m4v", // Apple M4V (MP4 variant)
                }
            )
            {
                if (contentType == type)
                    return true;
            }

            return false;
        }

        [GeneratedRegex(@"system=(\d+\.\d+)")]
        private static partial Regex MyRegex();
    }
}
