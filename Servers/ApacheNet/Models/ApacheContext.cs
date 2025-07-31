using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MultiServerLibrary.HTTP;
using WatsonWebserver.Core;

namespace ApacheNet.Models
{
    public class ApacheContext
    {
        public HttpContextBase Context { get; set; }
        public HttpRequestBase Request => Context.Request;
        public HttpResponseBase Response => Context.Response;
        public string ClientIP => Request.Source.IpAddress;
        public int ClientPort => Request.Source.Port;
        public string ServerIP => Request.Destination.IpAddress;
        public int ServerPort => Request.Destination.Port;
        public bool Secure => ServerPort.ToString().EndsWith("443");
        public bool AcceptChunked => ApacheNetServerConfiguration.HttpVersion.Equals("1.1") && ApacheNetServerConfiguration.ChunkedTransfers;

        public bool NoCompressCacheControl => Request.HeaderExists("Cache-Control") && Request.RetrieveHeaderValue("Cache-Control") == "no-transform";

        public DateTime CurrentDate => Request.Timestamp.Start;
        public HttpStatusCode StatusCode { get; set; }

        public string? FilePath { get; set; }
        public string? ApiPath { get; set; }
        public string? DirectoryPath { get; set; }
        public string AbsolutePath => HTTPProcessor.DecodeUrl(Request.Url.RawWithoutQuery);
        public string FullUrl => HTTPProcessor.DecodeUrl(Request.Url.RawWithQuery);

        public ApacheContext(HttpContextBase context)
        {
            Context = context;
        }

        public Task<bool> SendImmediate(bool chunked = false)
        {
            Response.ChunkedTransfer = chunked;
            Response.StatusCode = (int)StatusCode;
            return Response.Send();
        }

        public Task<bool> SendImmediate(string content, bool chunked = false)
        {
            Response.ChunkedTransfer = chunked;
            Response.StatusCode = (int)StatusCode;
            if (Response.ChunkedTransfer)
                return Response.SendChunk(Encoding.UTF8.GetBytes(content), true);
            return Response.Send(content);
        }

        public Task<bool> SendImmediate(byte[] content, bool chunked = false)
        {
            Response.ChunkedTransfer = chunked;
            Response.StatusCode = (int)StatusCode;
            if (Response.ChunkedTransfer)
                return Response.SendChunk(content, true);
            return Response.Send(content);
        }

        public async Task<bool> SendImmediate(Stream content, bool chunked = false)
        {
            Response.ChunkedTransfer = chunked;
            Response.StatusCode = (int)StatusCode;
            if (Response.ChunkedTransfer)
            {
                long bytesLeft = content.Length;

                if (bytesLeft == 0)
                    return await Response.SendChunk(Array.Empty<byte>(), true).ConfigureAwait(false);
                else
                {
                    const int buffersize = 16 * 1024;

                    bool isNotlastChunk;
                    byte[] buffer;

                    while (bytesLeft > 0)
                    {
                        isNotlastChunk = bytesLeft > buffersize;
                        buffer = new byte[isNotlastChunk ? buffersize : bytesLeft];
                        int n = await content.ReadAsync(buffer).ConfigureAwait(false);

                        if (isNotlastChunk)
                            await Response.SendChunk(buffer, false).ConfigureAwait(false);
                        else
                            return await Response.SendChunk(buffer, true).ConfigureAwait(false);

                        bytesLeft -= n;
                    }
                }
            }
            return await Response.Send(content.Length, content).ConfigureAwait(false);
        }

        public string GetHost()
        {
            string Host = Request.RetrieveHeaderValue("Host");
            if (string.IsNullOrEmpty(Host))
                Host = Request.RetrieveHeaderValue("HOST"); // Legacy format.
            return Host;
        }
    }
}
