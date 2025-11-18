using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ApacheNet.Models;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;

namespace ApacheNet
{
    public static class ApacheRequestHandler
    {
        public static Task<bool> HandleHEAD(ApacheContext ctx)
        {
            FileInfo? fileInfo = new(ctx.FilePath ?? string.Empty);
            if (fileInfo != null && fileInfo.Exists)
            {
                string ContentType = HTTPProcessor.GetMimeType(Path.GetExtension(ctx.FilePath), ApacheNetServerConfiguration.MimeTypes ?? HTTPProcessor.MimeTypes);
                if (ContentType == "application/octet-stream")
                {
                    bool matched = false;
                    byte[] VerificationChunck = FileSystemUtils.TryReadFileChunck(ctx.FilePath, 10, FileShare.ReadWrite, LocalFileStreamHelper.FileLockAwaitMs);
                    foreach (var entry in HTTPProcessor.PathernDictionary)
                    {
                        if (ByteUtils.FindBytePattern(VerificationChunck, entry.Value) != -1)
                        {
                            matched = true;
                            ctx.Response.ContentType = entry.Key;
                            break;
                        }
                    }
                    if (!matched)
                        ctx.Response.ContentType = ContentType;
                }
                else
                    ctx.Response.ContentType = ContentType;

                ctx.Response.Headers.Set("Content-Length", fileInfo.Length.ToString());
                ctx.Response.Headers.Set("Date", DateTime.Now.ToString("r"));
                ctx.Response.Headers.Set("Last-Modified", File.GetLastWriteTime(ctx.FilePath!).ToString("r"));
                ctx.Response.ContentLength = fileInfo.Length;
                ctx.StatusCode = HttpStatusCode.OK;
                return ctx.SendImmediate();
            }
            else
            {
                ctx.StatusCode = HttpStatusCode.NotFound;
                return ctx.SendImmediate();
            }
        }

        public static Task<bool> HandlePROPFIND(ApacheContext ctx)
        {
            if (File.Exists(ctx.FilePath))
            {
                const string httpVerIdent = "HTTP/";

                string ContentType = HTTPProcessor.GetMimeType(Path.GetExtension(ctx.FilePath), ApacheNetServerConfiguration.MimeTypes ?? HTTPProcessor.MimeTypes);
                if (ContentType == "application/octet-stream")
                {
                    byte[] VerificationChunck = FileSystemUtils.TryReadFileChunck(ctx.FilePath, 10, FileShare.ReadWrite, LocalFileStreamHelper.FileLockAwaitMs);
                    foreach (var entry in HTTPProcessor.PathernDictionary)
                    {
                        if (ByteUtils.FindBytePattern(VerificationChunck, entry.Value) != -1)
                        {
                            ContentType = entry.Key;
                            break;
                        }
                    }
                }

                string httpVer = ApacheNetServerConfiguration.HttpVersion;
                string serverIP = ctx.ServerIP;

                if (serverIP.Length > 15)
                    serverIP = "[" + serverIP + "]"; // Format the hostname if it's a IPV6 url format.

                ctx.StatusCode = HttpStatusCode.MultiStatus;
                ctx.Response.ContentType = "text/xml";
                return ctx.SendImmediate("<?xml version=\"1.0\"?>\r\n" +
                    "<a:multistatus\r\n" +
                    $"  xmlns:b=\"urn:uuid:{Guid.NewGuid()}/\"\r\n" +
                    "  xmlns:a=\"DAV:\">\r\n" +
                    " <a:response>\r\n" +
                    $"   <a:href>{(ctx.Secure ? "https" : "http")}://{serverIP}:{ctx.ServerPort}{ctx.AbsolutePath}</a:href>\r\n" +
                    "   <a:propstat>\r\n" +
                    $"    <a:status>{(httpVer.StartsWith(httpVerIdent, StringComparison.InvariantCultureIgnoreCase) ? httpVer : httpVerIdent + httpVer)} {(int)HttpStatusCode.OK} OK</a:status>\r\n" +
                    "       <a:prop>\r\n" +
                    $"        <a:getcontenttype>{ContentType}</a:getcontenttype>\r\n" +
                    $"        <a:getcontentlength b:dt=\"int\">{new FileInfo(ctx.FilePath).Length}</a:getcontentlength>\r\n" +
                    "       </a:prop>\r\n" +
                    "   </a:propstat>\r\n" +
                    " </a:response>\r\n" +
                    "</a:multistatus>");
            }
            else
            {
                ctx.StatusCode = HttpStatusCode.NotFound;
                return ctx.SendImmediate();
            }
        }
    }
}
