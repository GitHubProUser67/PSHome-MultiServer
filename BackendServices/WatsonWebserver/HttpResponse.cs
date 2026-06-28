using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MultiServerLibrary.Extension;
using SpaceWizards.HttpListener;
using WatsonWebserver.Core;

namespace WatsonWebserver
{
    /// <summary>
    /// HTTP response.
    /// </summary>
    public class HttpResponse : HttpResponseBase
    {
        #region Public-Members

        /// <summary>
        /// Retrieve the response body sent using a Send() or SendAsync() method.
        /// </summary>
        [JsonIgnore]
        public override string DataAsString
        {
            get
            {
                if (_DataAsBytes != null)
                    return Encoding.UTF8.GetString(_DataAsBytes);
                else if (_Data != null && ContentLength > 0)
                {
                    _DataAsBytes = ReadStreamFully(_Data);
                    if (_DataAsBytes != null)
                        return Encoding.UTF8.GetString(_DataAsBytes);
                }
                return null;
            }
        }

        /// <summary>
        /// Retrieve the response body sent using a Send() or SendAsync() method.
        /// </summary>
        [JsonIgnore]
        public override byte[] DataAsBytes
        {
            get
            {
                if (_DataAsBytes != null)
                    return _DataAsBytes;
                else if (_Data != null && ContentLength > 0)
                {
                    _DataAsBytes = ReadStreamFully(_Data);
                    return _DataAsBytes;
                }
                return null;
            }
        }

        /// <summary>
        /// Response data stream sent to the requestor.
        /// </summary>
        [JsonIgnore]
        public override MemoryStream Data
        {
            get
            {
                if (_Data == null)
                    throw new ArgumentNullException(nameof(_Data), "Input stream cannot be null");
                else if (!_Data.CanRead)
                    throw new NotSupportedException("Input stream is not readable");
                else if (!_Data.CanSeek)
                    throw new NotSupportedException("Input stream is not seekable");
                else if (_Data is MemoryStream data)
                    return data;

                var ms = new MemoryStream();

                if (ContentLength <= 0)
                    return ms;

                var dataPos = _Data.Position;

                try
                {
                    _Data.CopyTo(ms);
                }
                catch (Exception e)
                {
                    CustomLogger.LoggerAccessor.LogError(
                        $"[WatsonWebserver] - Data: an exception was thrown while copying data to the MemmoryStream: {e}"
                    );
                    ms.Clear();
                }

                _Data.Position = dataPos;

                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }

        #endregion

        #region Private-Members

        private readonly HttpListenerResponse _Response = null;
        private readonly Stream _OutputStream = null;

        private readonly HttpRequestBase _Request = null;
        private readonly HttpListenerContext _Context = null;
        private bool _Closed = false;
        private bool _HeadersSet = false;
        private readonly bool _KeepAliveData = true;

        private readonly WebserverSettings _Settings = new();
        private readonly WebserverEvents _Events = new();

        private readonly NameValueCollection _Headers = new(
            StringComparer.InvariantCultureIgnoreCase
        );
        private byte[] _DataAsBytes = null;
        private Stream _Data = null;
        private readonly ISerializationHelper _Serializer = null;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        internal HttpResponse(
            HttpRequestBase req,
            HttpListenerContext ctx,
            WebserverSettings settings,
            WebserverEvents events,
            ISerializationHelper serializer,
            bool KeepAliveResponseData
        )
        {
            _Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _Request = req ?? throw new ArgumentNullException(nameof(req));
            _Context = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _Response = _Context.Response;
            _Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _Events = events ?? throw new ArgumentNullException(nameof(events));
            _KeepAliveData = KeepAliveResponseData;

            _OutputStream = _Response.OutputStream;
        }

        #endregion

        #region Public-Methods

        public void Close()
        {
            if (_Closed)
                return;

            try
            {
                _OutputStream.Close();
            }
            catch { }

            _Response?.Close();

            _Closed = true;
        }

        /// <inheritdoc />
        public override async Task<bool> Send(CancellationToken token = default)
        {
            return ChunkedTransfer
                ? throw new IOException(
                    "Response is configured to use chunked transfer-encoding.  Use SendChunk() and SendFinalChunk()."
                )
                : await SendInternalAsync(0, null, token).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<bool> Send(long contentLength, CancellationToken token = default)
        {
            if (ChunkedTransfer)
                throw new IOException(
                    "Response is configured to use chunked transfer-encoding.  Use SendChunk() and SendFinalChunk()."
                );
            ContentLength = contentLength;
            return await SendInternalAsync(0, null, token).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<bool> Send(string data, CancellationToken token = default)
        {
            if (ChunkedTransfer)
                throw new IOException(
                    "Response is configured to use chunked transfer-encoding.  Use SendChunk() and SendFinalChunk()."
                );
            if (String.IsNullOrEmpty(data))
                return await SendInternalAsync(0, null, token).ConfigureAwait(false);

            var bytes = Encoding.UTF8.GetBytes(data);
            using (var ms = new MemoryStream(bytes))
                return await SendInternalAsync(bytes.Length, ms, token).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<bool> Send(byte[] data, CancellationToken token = default)
        {
            if (ChunkedTransfer)
                throw new IOException(
                    "Response is configured to use chunked transfer-encoding.  Use SendChunk() and SendFinalChunk()."
                );
            if (data == null || data.Length < 1)
                return await SendInternalAsync(0, null, token).ConfigureAwait(false);

            using (var ms = new MemoryStream(data))
                return await SendInternalAsync(data.Length, ms, token).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<bool> Send(
            long contentLength,
            Stream stream,
            CancellationToken token = default
        )
        {
            return ChunkedTransfer
                    ? throw new IOException(
                        "Response is configured to use chunked transfer-encoding.  Use SendChunk() and SendFinalChunk()."
                    )
                : stream == null || !stream.CanRead
                    ? await SendInternalAsync(0, null, token).ConfigureAwait(false)
                : await SendInternalAsync(contentLength, stream, token).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<bool> SendChunk(
            byte[] chunk,
            bool isFinal,
            CancellationToken token = default
        )
        {
            if (!ChunkedTransfer)
                throw new IOException(
                    "Response is not configured to use chunked transfer-encoding.  Set ChunkedTransfer to true first, otherwise use Send()."
                );
            else if (!_HeadersSet)
                SendHeaders();

            if (chunk != null && chunk.Length > 0)
                ContentLength += chunk.Length;

            try
            {
                // When SendChunked = true, http.sys expects us to write raw chunk data
                // and it will handle the chunked encoding format automatically
                if (chunk != null && chunk.Length > 0)
                    await _OutputStream.WriteAsync(chunk, token).ConfigureAwait(false);

                await _OutputStream.FlushAsync(token).ConfigureAwait(false);

                if (isFinal)
                {
                    // For http.sys, we need to close the stream to signal the final chunk
                    // http.sys will automatically send the "0\r\n\r\n" final chunk marker
                    Close();
                    ResponseSent = true;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override async Task<bool> SendEvent(
            ServerSentEvent sse,
            bool isFinal,
            CancellationToken token = default
        )
        {
            if (!ServerSentEvents)
                throw new IOException(
                    "Response is not configured to use server-sent events.  Set ServerSentEvents to true first, otherwise use Send()."
                );
            if (!_HeadersSet)
                SendHeaders();
            ArgumentNullException.ThrowIfNull(sse);

            var msg = sse.ToEventString();
            if (string.IsNullOrEmpty(msg))
                throw new ArgumentException(
                    "A null or unpopulated server-sent event object was supplied."
                );

            try
            {
                await _OutputStream
                    .WriteAsync(Encoding.UTF8.GetBytes(msg), token)
                    .ConfigureAwait(false);
                await _OutputStream.FlushAsync(token).ConfigureAwait(false);

                if (isFinal)
                {
                    Close();
                    ResponseSent = true;
                }

                return true;
            }
            catch
            {
                // Not Important.
            }

            return false;
        }

        /// <summary>
        /// Dispose of resources.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_Data != null)
                {
                    try
                    {
                        _Data.Dispose();
                    }
                    catch { }
                    _Data = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private-Methods

        private static string GetStatusDescription(int statusCode)
        {
            // Helpful links:
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status
            // https://en.wikipedia.org/wiki/List_of_HTTP_status_codes

            return statusCode switch
            {
                100 => "Continue",
                101 => "Switching Protocols",
                102 => "Processing",
                103 => "Early Hints",
                200 => "OK",
                201 => "Created",
                202 => "Accepted",
                203 => "Non-Authoritative Information",
                204 => "No Content",
                205 => "Reset Content",
                206 => "Partial Content",
                207 => "Multi-Status",
                208 => "Already Reported",
                226 => "IM Used",
                300 => "Multiple Choices",
                301 => "Moved Permanently",
                302 => "Moved Temporarily",
                303 => "See Other",
                304 => "Not Modified",
                305 => "Use Proxy",
                306 => "Switch Proxy",
                307 => "Temporary Redirect",
                308 => "Permanent Redirect",
                400 => "Bad Request",
                401 => "Unauthorized",
                402 => "Payment Required",
                403 => "Forbidden",
                404 => "Not Found",
                405 => "Method Not Allowed",
                406 => "Not Acceptable",
                407 => "Proxy Authentication Required",
                408 => "Request Timeout",
                409 => "Conflict",
                410 => "Gone",
                411 => "Length Required",
                412 => "Precondition Failed",
                413 => "Payload too Large",
                414 => "URI Too Long",
                415 => "Unsupported Media Type",
                416 => "Range Not Satisfiable",
                417 => "Expectation Failed",
                418 => "I'm a teapot",
                421 => "Misdirected Request",
                422 => "Unprocessable Content",
                423 => "Locked",
                424 => "Failed Dependency",
                425 => "Too Early",
                426 => "Upgrade Required",
                428 => "Precondition Required",
                429 => "Too Many Requests",
                431 => "Request Header Fields Too Large",
                451 => "Unavailable For Legal Reasons",
                500 => "Internal Server Error",
                501 => "Not Implemented",
                502 => "Bad Gateway",
                503 => "Service Unavailable",
                504 => "Gateway Timeout",
                505 => "HTTP Version Not Supported",
                506 => "Variant Also Negotiates",
                507 => "Insufficient Storage",
                508 => "Loop Detected",
                510 => "Not Extended",
                511 => "Network Authentication Required",
                _ => "Unknown",
            };
        }

        private void SendHeaders()
        {
            if (_HeadersSet)
                throw new IOException("Headers already sent.");

            _Response.ProtocolVersion = new Version(1, 1);
            _Response.ContentLength64 = ContentLength;
            _Response.StatusCode = StatusCode;
            _Response.StatusDescription = GetStatusDescription(StatusCode);
            _Response.SendChunked = ChunkedTransfer || ServerSentEvents;
            _Response.ContentType = ContentType;
            _Response.KeepAlive = _Settings.IO.EnableKeepAlive;

            if (ServerSentEvents)
            {
                _Response.ContentType = "text/event-stream; charset=utf-8";
                _Response.Headers.Add("Cache-Control", "no-cache");
                _Response.Headers.Add("Connection", "keep-alive");
            }

            if (Headers != null && Headers.Count > 0)
            {
                for (var i = 0; i < Headers.Count; i++)
                {
                    var key = Headers.GetKey(i);
                    var vals = Headers.GetValues(i);

                    if (vals == null || vals.Length < 1)
                    {
                        _Response.AddHeader(key, null);
                    }
                    else
                    {
                        for (var j = 0; j < vals.Length; j++)
                        {
                            _Response.AddHeader(key, vals[j]);
                        }
                    }
                }
            }

            if (
                _Settings.Headers.DefaultHeaders != null
                && _Settings.Headers.DefaultHeaders.Count > 0
            )
            {
                foreach (var header in _Settings.Headers.DefaultHeaders)
                {
                    if (Headers.Get(header.Key) != null || Headers.AllKeys.Contains(header.Key))
                    {
                        // already present
                    }
                    else
                    {
                        _Response.AddHeader(header.Key, header.Value);
                    }
                }
            }

            _HeadersSet = true;
        }

        private static byte[] ReadStreamFully(Stream input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (!input.CanRead)
                throw new InvalidOperationException("Input stream is not readable");

            using (var ms = new MemoryStream())
            {
                StreamUtils.CopyStream(input, ms);

                return ms.ToArray();
            }
        }

        private async Task<bool> SendInternalAsync(
            long contentLength,
            Stream stream,
            CancellationToken token = default
        )
        {
            if (ChunkedTransfer)
                throw new IOException(
                    "Response is configured to use chunked transfer-encoding.  Use SendChunk() and SendFinalChunk()."
                );

            if (ContentLength == 0 && contentLength > 0)
                ContentLength = contentLength;

            if (!_HeadersSet)
                SendHeaders();

            try
            {
                if (_Request.Method != HttpMethod.HEAD)
                {
                    if (stream != null && stream.CanRead)
                    {
                        var bufferSize = _Settings.IO.StreamBufferSize;

                        // Some clients might cut the connection while the data is being copied, this is expected, so we simply ignore failed writes.
                        if (ContentLength > 0)
                        {
                            if (_KeepAliveData)
                            {
                                int bytesRead;
                                var bytesRemaining = contentLength;

                                var buffer = new byte[bufferSize];

                                _Data = new MemoryStream();

                                while (bytesRemaining > 0)
                                {
                                    bytesRead = await stream
                                        .ReadAsync(buffer, token)
                                        .ConfigureAwait(false);
                                    if (bytesRead > 0)
                                    {
                                        await _Data
                                            .WriteAsync(buffer.AsMemory(0, bytesRead), token)
                                            .ConfigureAwait(false);
                                        await _OutputStream
                                            .WriteAsync(buffer.AsMemory(0, bytesRead), token)
                                            .ConfigureAwait(false);
                                        bytesRemaining -= bytesRead;
                                    }
                                }

                                _Data.Seek(0, SeekOrigin.Begin);
                            }
                            else
                                await StreamUtils
                                    .CopyStreamAsync(
                                        stream,
                                        _OutputStream,
                                        bufferSize,
                                        ContentLength,
                                        false,
                                        token
                                    )
                                    .ConfigureAwait(false);
                        }
                        else
                            await StreamUtils
                                .CopyStreamAsync(stream, _OutputStream, bufferSize, false, token)
                                .ConfigureAwait(false);

                        await _OutputStream.FlushAsync(token).ConfigureAwait(false);
                    }
                }

                Close();

                ResponseSent = true;
                return true;
            }
            catch
            {
                if (_Data != null)
                {
                    try
                    {
                        _Data.Dispose();
                    }
                    catch
                    {
                        // Not Important.
                    }
                    _Data = null;
                }

                return false;
            }
        }

        #endregion
    }
}
