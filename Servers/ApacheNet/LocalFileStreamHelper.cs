using System.Net;
using System.Text;
using WatsonWebserver.Core;
using NetworkLibrary.HTTP;
using System.IO;
using System;
using System.Threading.Tasks;
using NetworkLibrary.Extension;
using NetworkLibrary.Upscalers;

namespace ApacheNet
{
    public class LocalFileStreamHelper
    {
        public const int FileLockAwaitMs = 500;

        public const long compressionSizeLimit = 800L * 1024 * 1024; // 800MB in bytes

        public static async Task<bool> HandleRequest(HttpContextBase ctx, string encoding, string absolutepath, string filePath, string ContentType, bool isVideoOrAudio, bool isHtmlCompatible, bool noCompressCacheControl)
        {
            bool isNoneMatchValid = false;
            string ifModifiedSince = ctx.Request.RetrieveHeaderValue("If-Modified-Since");
            bool isModifiedSinceValid = HTTPProcessor.CheckLastWriteTime(filePath, ifModifiedSince);
            string NoneMatch = ctx.Request.RetrieveHeaderValue("If-None-Match");
            string EtagMD5 = HTTPProcessor.ETag(filePath);

            if (!string.IsNullOrEmpty(EtagMD5))
            {
                isNoneMatchValid = NoneMatch == EtagMD5;
                ctx.Response.Headers.Add("ETag", EtagMD5);
                ctx.Response.Headers.Add("Expires", DateTime.Now.AddMinutes(30).ToString("r"));
            }

            if ((isNoneMatchValid && isModifiedSinceValid) ||
                (isNoneMatchValid && string.IsNullOrEmpty(ifModifiedSince)) ||
                (isModifiedSinceValid && string.IsNullOrEmpty(NoneMatch)))
            {
                ctx.Response.ChunkedTransfer = false;
                ctx.Response.ContentType = "text/plain";
                ctx.Response.StatusCode = (int)HttpStatusCode.NotModified;
                return await ctx.Response.Send().ConfigureAwait(false);
            }

            bool compressionSettingEnabled = ApacheNetServerConfiguration.EnableHTTPCompression;
            bool sent = false;
            string extension = Path.GetExtension(filePath);
            Stream? st;

            if (ApacheNetServerConfiguration.EnableImageUpscale && ((!string.IsNullOrEmpty(ContentType) && ContentType.StartsWith("image/")) || (!string.IsNullOrEmpty(extension) && extension.Equals(".dds", StringComparison.InvariantCultureIgnoreCase))))
            {
                ctx.Response.ContentType = ContentType;

                try
                {
                    st = ImageOptimizer.OptimizeImage(ApacheNetServerConfiguration.ConvertersFolder, filePath, extension, ImageOptimizer.defaultOptimizerParams);

                    if (compressionSettingEnabled && !noCompressCacheControl && !string.IsNullOrEmpty(encoding) && st.Length <= compressionSizeLimit)
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
                // Generate an HTML page with the video element
                const string htmlPart = @"
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
                                  <source src=""";
                string htmlContent = htmlPart + absolutepath + $@""" type=""{ContentType}"">
                                </video>
                              </div>
                            </body>
                            </html>";

                ctx.Response.ContentType = "text/html; charset=UTF-8";

                MemoryStream htmlMs = new MemoryStream(Encoding.UTF8.GetBytes(htmlContent));

                if (compressionSettingEnabled && !noCompressCacheControl && !string.IsNullOrEmpty(encoding))
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

                if (compressionSettingEnabled && !noCompressCacheControl && !string.IsNullOrEmpty(encoding) && new FileInfo(filePath).Length <= compressionSizeLimit)
                {
                    if (encoding.Contains("zstd"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "zstd");
                        st = HTTPProcessor.ZstdCompressStream(await FileSystemUtils.TryOpen(filePath, FileSystemUtils.FileAccessMode.Read, FileLockAwaitMs).ConfigureAwait(false));
                    }
                    else if (encoding.Contains("br"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "br");
                        st = HTTPProcessor.BrotliCompressStream(await FileSystemUtils.TryOpen(filePath, FileSystemUtils.FileAccessMode.Read, FileLockAwaitMs).ConfigureAwait(false));
                    }
                    else if (encoding.Contains("gzip"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "gzip");
                        st = HTTPProcessor.GzipCompressStream(await FileSystemUtils.TryOpen(filePath, FileSystemUtils.FileAccessMode.Read, FileLockAwaitMs).ConfigureAwait(false));
                    }
                    else if (encoding.Contains("deflate"))
                    {
                        ctx.Response.Headers.Add("Content-Encoding", "deflate");
                        st = HTTPProcessor.DeflateStream(await FileSystemUtils.TryOpen(filePath, FileSystemUtils.FileAccessMode.Read, FileLockAwaitMs).ConfigureAwait(false));
                    }
                    else
                        st = await FileSystemUtils.TryOpen(filePath, FileSystemUtils.FileAccessMode.Read, FileLockAwaitMs).ConfigureAwait(false);
                }
                else
                    st = await FileSystemUtils.TryOpen(filePath, FileSystemUtils.FileAccessMode.Read, FileLockAwaitMs).ConfigureAwait(false);
            }

            if (st == null)
            {
                ctx.Response.ChunkedTransfer = false;
                ctx.Response.Headers.Clear();
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                ctx.Response.ContentType = "text/plain";
                return await ctx.Response.Send().ConfigureAwait(false);
            }

            using (st)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                ctx.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(filePath).ToString("r"));
                if (ctx.Response.ChunkedTransfer)
                {
                    long bytesLeft = st.Length;

                    if (bytesLeft == 0)
                        sent = await ctx.Response.SendChunk(Array.Empty<byte>(), true).ConfigureAwait(false);
                    else
                    {
                        const int buffersize = 16 * 1024;

                        bool isNotlastChunk;
                        byte[] buffer;

                        while (bytesLeft > 0)
                        {
                            isNotlastChunk = bytesLeft > buffersize;
                            buffer = new byte[isNotlastChunk ? buffersize : bytesLeft];
                            int n = await st.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                            if (isNotlastChunk)
                                await ctx.Response.SendChunk(buffer, false).ConfigureAwait(false);
                            else
                                sent = await ctx.Response.SendChunk(buffer, true).ConfigureAwait(false);

                            bytesLeft -= n;
                        }
                    }
                }
                else
                    sent = await ctx.Response.Send(st.Length, st).ConfigureAwait(false);
            }

            return sent;
        }

        public static async Task<bool> HandlePartialRangeRequest(HttpContextBase ctx, string filePath, string ContentType,
            bool noCompressCacheControl, string boundary = "multiserver_separator")
        {
            // This method directly communicate with the wire to handle, normally, imposible transfers.
            // If a part of the code sounds weird to you, it's normal... So does curl tests...

            if (HTTPProcessor.CheckLastWriteTime(filePath, ctx.Request.RetrieveHeaderValue("If-Unmodified-Since"), true))
            {
                ctx.Response.ChunkedTransfer = false;
                ctx.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                return await ctx.Response.Send().ConfigureAwait(false);
            }
            else
            {
                try
                {
                    FileStream fs = await FileSystemUtils.TryOpen(filePath, FileSystemUtils.FileAccessMode.Read, FileLockAwaitMs).ConfigureAwait(false);

                    if (fs != null)
                    {
                        const int rangebuffersize = 32768;

                        string acceptencoding = ctx.Request.RetrieveHeaderValue("Accept-Encoding");

                        using (fs)
                        {
                            long startByte = -1;
                            long endByte = -1;
                            long filesize = fs.Length;
                            string HeaderString = ctx.Request.RetrieveHeaderValue("Range").Replace("bytes=", string.Empty);
                            if (HeaderString.Contains(','))
                            {
                                byte[] multipartSeparator = Encoding.UTF8.GetBytes($"--{boundary}--");
                                byte[] Separator = new byte[] { 0x0D, 0x0A };

                                using HugeMemoryStream ms = new();
                                // Split the ranges based on the comma (',') separator
                                foreach (string RangeSelect in HeaderString.Split(','))
                                {
                                    byte[] contentTypeBytes = Encoding.UTF8.GetBytes($"Content-Type: {ContentType}");
                                    ms.Write(Separator, 0, Separator.Length);
                                    ms.Write(multipartSeparator, 0, multipartSeparator.Length - 2);
                                    ms.Write(Separator, 0, Separator.Length);
                                    ms.Write(contentTypeBytes, 0, contentTypeBytes.Length);
                                    ms.Write(Separator, 0, Separator.Length);
                                    fs.Position = 0;
                                    startByte = -1;
                                    endByte = -1;
                                    string[] range = RangeSelect.Split('-');
                                    if (range[0].Trim().Length > 0) _ = long.TryParse(range[0], out startByte);
                                    if (range[1].Trim().Length > 0) _ = long.TryParse(range[1], out endByte);
                                    if (endByte == -1) endByte = filesize;
                                    else if (endByte != filesize) endByte++;
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
                                        const string payload = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\r\n" +
                                                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"\r\n" +
                                                "         \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n" +
                                                "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">\r\n" +
                                                "        <head>\r\n" +
                                                "                <title>416 - Requested Range Not Satisfiable</title>\r\n" +
                                                "        </head>\r\n" +
                                                "        <body>\r\n" +
                                                "                <h1>416 - Requested Range Not Satisfiable</h1>\r\n" +
                                                "        </body>\r\n" +
                                                "</html>";

                                        ms.Flush();
                                        ms.Close();
                                        fs.Flush();
                                        fs.Close();
                                        ctx.Response.Headers.Add("Content-Range", string.Format("bytes */{0}", filesize));
                                        ctx.Response.StatusCode = (int)HttpStatusCode.RequestedRangeNotSatisfiable;
                                        ctx.Response.ContentType = "text/html; charset=UTF-8";
                                        if (ApacheNetServerConfiguration.EnableHTTPCompression && !noCompressCacheControl && !string.IsNullOrEmpty(acceptencoding))
                                        {
                                            if (acceptencoding.Contains("zstd"))
                                            {
                                                ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                                payloadBytes = HTTPProcessor.CompressZstd(Encoding.UTF8.GetBytes(payload));
                                            }
                                            else if (acceptencoding.Contains("br"))
                                            {
                                                ctx.Response.Headers.Add("Content-Encoding", "br");
                                                payloadBytes = HTTPProcessor.CompressBrotli(Encoding.UTF8.GetBytes(payload));
                                            }
                                            else if (acceptencoding.Contains("gzip"))
                                            {
                                                ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                                payloadBytes = HTTPProcessor.CompressGzip(Encoding.UTF8.GetBytes(payload));
                                            }
                                            else if (acceptencoding.Contains("deflate"))
                                            {
                                                ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                                payloadBytes = HTTPProcessor.Deflate(Encoding.UTF8.GetBytes(payload));
                                            }
                                            else
                                                payloadBytes = Encoding.UTF8.GetBytes(payload);
                                        }
                                        else
                                            payloadBytes = Encoding.UTF8.GetBytes(payload);

                                        if (ctx.Response.ChunkedTransfer)
                                            return await ctx.Response.SendChunk(payloadBytes, true).ConfigureAwait(false);
                                        else
                                            return await ctx.Response.Send(payloadBytes).ConfigureAwait(false);
                                    }
                                    else if ((startByte >= endByte) || startByte < 0 || endByte <= 0) // Curl test showed this behaviour.
                                    {
                                        ms.Flush();
                                        ms.Close();
                                        fs.Position = 0;

                                        ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        ctx.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(filePath).ToString("r"));
                                        ctx.Response.Headers.Add("Accept-Ranges", "bytes");
                                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                        ctx.Response.ContentType = ContentType;

                                        if (ctx.Response.ChunkedTransfer)
                                        {
                                            long bytesLeft = new FileInfo(filePath).Length;

                                            if (bytesLeft == 0)
                                                return await ctx.Response.SendChunk(Array.Empty<byte>(), true).ConfigureAwait(false);

                                            const int buffersize = 16 * 1024;

                                            bool isNotlastChunk;
                                            byte[] buffer;

                                            while (bytesLeft > 0)
                                            {
                                                isNotlastChunk = bytesLeft > buffersize;
                                                buffer = new byte[isNotlastChunk ? buffersize : bytesLeft];
                                                int n = await fs.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                                                if (isNotlastChunk)
                                                    await ctx.Response.SendChunk(buffer, false).ConfigureAwait(false);
                                                else
                                                    return await ctx.Response.SendChunk(buffer, true).ConfigureAwait(false);

                                                bytesLeft -= n;
                                            }
                                        }
                                        else
                                            return await ctx.Response.Send(new FileInfo(filePath).Length, fs);
                                    }
                                    else
                                    {
                                        int bytesRead = 0;
                                        long TotalBytes = endByte - startByte;
                                        long totalBytesCopied = 0;
                                        byte[] buffer = new byte[rangebuffersize];
                                        byte[] contentRangeBytes = Encoding.UTF8.GetBytes("Content-Range: " + string.Format("bytes {0}-{1}/{2}", startByte, endByte - 1, filesize));
                                        fs.Position = startByte;
                                        ms.Write(contentRangeBytes, 0, contentRangeBytes.Length);
                                        ms.Write(Separator, 0, Separator.Length);
                                        ms.Write(Separator, 0, Separator.Length);
                                        while (totalBytesCopied < TotalBytes && (bytesRead = await fs.ReadAsync(buffer, 0, rangebuffersize).ConfigureAwait(false)) > 0)
                                        {
                                            int bytesToWrite = (int)Math.Min(TotalBytes - totalBytesCopied, bytesRead);
                                            ms.Write(buffer, 0, bytesToWrite);
                                            totalBytesCopied += bytesToWrite;
                                        }
                                    }
                                }
                                ms.Write(Separator, 0, Separator.Length);
                                ms.Write(multipartSeparator, 0, multipartSeparator.Length);
                                ms.Write(Separator, 0, Separator.Length);
                                ms.Position = 0;
                                ctx.Response.ContentType = $"multipart/byteranges; boundary={boundary}";
                                ctx.Response.Headers.Add("Accept-Ranges", "bytes");
                                ctx.Response.Headers.Add("Content-Length", ms.Length.ToString());
                                ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                ctx.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(filePath).ToString("r"));
                                ctx.Response.StatusCode = (int)HttpStatusCode.PartialContent;

                                fs.Flush();
                                fs.Close();

                                if (ctx.Response.ChunkedTransfer)
                                {
                                    long bytesLeft = ms.Length;

                                    if (bytesLeft == 0)
                                        return await ctx.Response.SendChunk(Array.Empty<byte>(), true).ConfigureAwait(false);

                                    const int buffersize = 16 * 1024;

                                    bool isNotlastChunk;
                                    byte[] buffer;

                                    while (bytesLeft > 0)
                                    {
                                        isNotlastChunk = bytesLeft > buffersize;
                                        buffer = new byte[isNotlastChunk ? buffersize : bytesLeft];
                                        int n = await ms.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                                        if (isNotlastChunk)
                                            await ctx.Response.SendChunk(buffer, false).ConfigureAwait(false);
                                        else
                                            return await ctx.Response.SendChunk(buffer, true).ConfigureAwait(false);

                                        bytesLeft -= n;
                                    }
                                }
                                else
                                    return await ctx.Response.Send(ms.Length, ms).ConfigureAwait(false);
                            }
                            else
                            {
                                string[] range = HeaderString.Split('-');
                                if (range[0].Trim().Length > 0) _ = long.TryParse(range[0], out startByte);
                                if (range[1].Trim().Length > 0) _ = long.TryParse(range[1], out endByte);
                                if (endByte == -1) endByte = filesize;
                                else if (endByte != filesize) endByte++;
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
                                const string payload = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\r\n" +
                                                    "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"\r\n" +
                                                    "         \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n" +
                                                    "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">\r\n" +
                                                    "        <head>\r\n" +
                                                    "                <title>416 - Requested Range Not Satisfiable</title>\r\n" +
                                                    "        </head>\r\n" +
                                                    "        <body>\r\n" +
                                                    "                <h1>416 - Requested Range Not Satisfiable</h1>\r\n" +
                                                    "        </body>\r\n" +
                                                    "</html>";

                                fs.Flush();
                                fs.Close();
                                ctx.Response.Headers.Add("Content-Range", string.Format("bytes */{0}", filesize));
                                ctx.Response.StatusCode = (int)HttpStatusCode.RequestedRangeNotSatisfiable;
                                ctx.Response.ContentType = "text/html; charset=UTF-8";
                                if (ApacheNetServerConfiguration.EnableHTTPCompression && !noCompressCacheControl && !string.IsNullOrEmpty(acceptencoding))
                                {
                                    if (acceptencoding.Contains("zstd"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "zstd");
                                        payloadBytes = HTTPProcessor.CompressZstd(Encoding.UTF8.GetBytes(payload));
                                    }
                                    else if (acceptencoding.Contains("br"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "br");
                                        payloadBytes = HTTPProcessor.CompressBrotli(Encoding.UTF8.GetBytes(payload));
                                    }
                                    else if (acceptencoding.Contains("gzip"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "gzip");
                                        payloadBytes = HTTPProcessor.CompressGzip(Encoding.UTF8.GetBytes(payload));
                                    }
                                    else if (acceptencoding.Contains("deflate"))
                                    {
                                        ctx.Response.Headers.Add("Content-Encoding", "deflate");
                                        payloadBytes = HTTPProcessor.Deflate(Encoding.UTF8.GetBytes(payload));
                                    }
                                    else
                                        payloadBytes = Encoding.UTF8.GetBytes(payload);
                                }
                                else
                                    payloadBytes = Encoding.UTF8.GetBytes(payload);

                                if (ctx.Response.ChunkedTransfer)
                                    return await ctx.Response.SendChunk(payloadBytes, true).ConfigureAwait(false);
                                else
                                    return await ctx.Response.Send(payloadBytes).ConfigureAwait(false);
                            }
                            else if ((startByte >= endByte) || startByte < 0 || endByte <= 0) // Curl test showed this behaviour.
                            {
                                fs.Position = 0;

                                ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                ctx.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(filePath).ToString("r"));
                                ctx.Response.Headers.Add("Accept-Ranges", "bytes");
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = ContentType;

                                if (ctx.Response.ChunkedTransfer)
                                {
                                    long bytesLeft = new FileInfo(filePath).Length;

                                    if (bytesLeft == 0)
                                        return await ctx.Response.SendChunk(Array.Empty<byte>(), true).ConfigureAwait(false);

                                    const int buffersize = 16 * 1024;

                                    bool isNotlastChunk;
                                    byte[] buffer;

                                    while (bytesLeft > 0)
                                    {
                                        isNotlastChunk = bytesLeft > buffersize;
                                        buffer = new byte[isNotlastChunk ? buffersize : bytesLeft];
                                        int n = await fs.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                                        if (isNotlastChunk)
                                            await ctx.Response.SendChunk(buffer, false).ConfigureAwait(false);
                                        else
                                            return await ctx.Response.SendChunk(buffer, true).ConfigureAwait(false);

                                        bytesLeft -= n;
                                    }
                                }
                                else
                                    return await ctx.Response.Send(new FileInfo(filePath).Length, fs).ConfigureAwait(false);
                            }
                            else
                            {
                                long TotalBytes = endByte - startByte;
                                fs.Position = startByte;
                                ctx.Response.ContentType = ContentType;
                                ctx.Response.Headers.Add("Accept-Ranges", "bytes");
                                ctx.Response.Headers.Add("Content-Range", string.Format("bytes {0}-{1}/{2}", startByte, endByte - 1, filesize));
                                ctx.Response.Headers.Add("Content-Length", TotalBytes.ToString());
                                ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                ctx.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(filePath).ToString("r"));
                                ctx.Response.StatusCode = (int)HttpStatusCode.PartialContent;

                                if (ctx.Response.ChunkedTransfer)
                                {
                                    if (TotalBytes == 0)
                                        return await ctx.Response.SendChunk(Array.Empty<byte>(), true).ConfigureAwait(false);

                                    const int buffersize = 16 * 1024;

                                    bool isNotlastChunk;
                                    byte[] buffer;

                                    while (TotalBytes > 0)
                                    {
                                        isNotlastChunk = TotalBytes > buffersize;
                                        buffer = new byte[isNotlastChunk ? buffersize : TotalBytes];
                                        int n = await fs.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

                                        if (isNotlastChunk)
                                            await ctx.Response.SendChunk(buffer, false).ConfigureAwait(false);
                                        else
                                            return await ctx.Response.SendChunk(buffer, true).ConfigureAwait(false);

                                        TotalBytes -= n;
                                    }
                                }
                                else
                                    return await ctx.Response.Send(TotalBytes, fs).ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch
                {
                }

                ctx.Response.ChunkedTransfer = false;
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                ctx.Response.ContentType = "text/plain";
                return await ctx.Response.Send().ConfigureAwait(false);
            }
        }
    }
}
