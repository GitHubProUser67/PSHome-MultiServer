using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MultiServerLibrary.Extension;
using WatsonWebserver.Core;

namespace WatsonWebserver
{
    /// <summary>
    /// HTTP request.
    /// </summary>
    public class HttpRequestNative : HttpRequestBase
    {
        #region Public-Members

        /// <summary>
        /// The stream from which to read the request body sent by the requestor (client).
        /// </summary>
        [JsonIgnore]
        public override Stream Data { get; set; } = null;

        /// <summary>
        /// Retrieve the request body as a byte array.  This will fully read the stream.
        /// </summary>
        [JsonIgnore]
        public override byte[] DataAsBytes
        {
            get
            {
                if (_DataAsBytes != null)
                    return _DataAsBytes;
                else if (Data != null && ContentLength > 0)
                {
                    _DataAsBytes = ReadStreamFully(Data);
                    return _DataAsBytes;
                }
                return null;
            }
        }

        /// <summary>
        /// Retrieve the request body as a string.  This will fully read the stream.
        /// </summary>
        [JsonIgnore]
        public override string DataAsString
        {
            get
            {
                if (_DataAsBytes != null)
                    return Encoding.UTF8.GetString(_DataAsBytes);
                else if (Data != null && ContentLength > 0)
                {
                    _DataAsBytes = ReadStreamFully(Data);
                    if (_DataAsBytes != null)
                        return Encoding.UTF8.GetString(_DataAsBytes);
                }
                return null;
            }
        }

        /// <summary>
        /// The original HttpListenerContext from which the HttpRequest was constructed.
        /// </summary>
        [JsonIgnore]
        public HttpListenerContext ListenerContext { get; set; }

        #endregion

        #region Private-Members

        private readonly int _StreamBufferSize = 65536;
        private readonly Uri _Uri = null;
        private byte[] _DataAsBytes = null;
        private readonly ISerializationHelper _Serializer = null;
        private readonly NameValueCollection _Headers = new(
            StringComparer.InvariantCultureIgnoreCase
        );

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// HTTP request.
        /// Instantiate the object using an HttpListenerContext.
        /// </summary>
        /// <param name="ctx">HttpListenerContext.</param>
        /// <param name="serializer">Serialization helper.</param>
        public HttpRequestNative(HttpListenerContext ctx, ISerializationHelper serializer)
        {
            ArgumentNullException.ThrowIfNull(ctx);
            if (ctx.Request == null)
                throw new ArgumentNullException(nameof(ctx.Request));
            _Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            ListenerContext = ctx;
            Keepalive = ctx.Request.KeepAlive;
            ContentLength = ctx.Request.ContentLength64;
            Useragent = ctx.Request.UserAgent;
            ContentType = ctx.Request.ContentType;

            _Uri = new Uri(ctx.Request.Url.ToString().Trim());

            ProtocolVersion = "HTTP/" + ctx.Request.ProtocolVersion.ToString();
            Source = new SourceDetails(
                ctx.Request.RemoteEndPoint.Address.ToString(),
                ctx.Request.RemoteEndPoint.Port
            );
            Destination = new DestinationDetails(
                ctx.Request.LocalEndPoint.Address.ToString(),
                ctx.Request.LocalEndPoint.Port,
                _Uri.Host
            );
            Url = new UrlDetails(
                ctx.Request.Url.ToString().Trim(),
                ctx.Request.RawUrl.ToString().Trim()
            );
            Query = new QueryDetails(Url.Full);
            MethodRaw = ctx.Request.HttpMethod;

            try
            {
                Method = Enum.Parse<HttpMethod>(ctx.Request.HttpMethod, true);
            }
            catch (Exception)
            {
                Method = HttpMethod.UNKNOWN;
            }

            Headers = ctx.Request.Headers;

            for (var i = 0; i < Headers.Count; i++)
            {
                var key = Headers.GetKey(i);
                var vals = Headers.GetValues(i);

                if (string.IsNullOrEmpty(key))
                    continue;
                else if (vals == null || vals.Length < 1)
                    continue;

                if (key.ToLower().Equals("transfer-encoding"))
                {
                    if (vals.Contains("chunked", StringComparer.InvariantCultureIgnoreCase))
                        ChunkedTransfer = true;
                    if (vals.Contains("gzip", StringComparer.InvariantCultureIgnoreCase))
                        Gzip = true;
                    if (vals.Contains("deflate", StringComparer.InvariantCultureIgnoreCase))
                        Deflate = true;
                }
                else if (key.ToLower().Equals("x-amz-content-sha256"))
                {
                    if (vals.Contains("streaming", StringComparer.InvariantCultureIgnoreCase))
                        ChunkedTransfer = true;
                }
            }

            Data = ctx.Request.InputStream;
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// For chunked transfer-encoded requests, read the next chunk.
        /// It is strongly recommended that you use the ChunkedTransfer parameter before invoking this method.
        /// </summary>
        /// <param name="token">Cancellation token useful for canceling the request.</param>
        /// <returns>Chunk.</returns>
        public override async Task<Chunk> ReadChunk(CancellationToken token = default)
        {
            var chunk = new Chunk();

            #region Get-Length-and-Metadata

            var buffer = new byte[1];
            byte[] lenBytes = null;

            int bytesRead;
            while (true)
            {
                bytesRead = await Data.ReadAsync(buffer, token).ConfigureAwait(false);
                if (bytesRead > 0)
                {
                    lenBytes = ByteUtils.CombineByteArray(lenBytes, buffer);
                    var lenStr = Encoding.UTF8.GetString(lenBytes);

                    if (lenBytes[^1] == 10)
                    {
                        lenStr = lenStr.Trim();

                        if (lenStr.Contains(';'))
                        {
                            var lenParts = lenStr.Split([';'], 2);
                            chunk.Length = int.Parse(lenParts[0], NumberStyles.HexNumber);
                            if (lenParts.Length >= 2)
                                chunk.Metadata = lenParts[1];
                        }
                        else
                            chunk.Length = int.Parse(lenStr, NumberStyles.HexNumber);

                        break;
                    }
                }
            }

            #endregion

            #region Get-Data

            var bytesRemaining = chunk.Length;

            if (chunk.Length > 0)
            {
                chunk.IsFinal = false;
                using (var ms = new MemoryStream())
                {
                    while (true)
                    {
                        buffer =
                            bytesRemaining > _StreamBufferSize
                                ? (new byte[_StreamBufferSize])
                                : (new byte[bytesRemaining]);

                        bytesRead = await Data.ReadAsync(buffer, token).ConfigureAwait(false);

                        if (bytesRead > 0)
                        {
                            await ms.WriteAsync(buffer.AsMemory(0, bytesRead), token)
                                .ConfigureAwait(false);
                            bytesRemaining -= bytesRead;
                        }

                        if (bytesRemaining == 0)
                            break;
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    chunk.Data = ms.ToArray();
                }
            }
            else
            {
                chunk.IsFinal = true;
            }

            #endregion

            #region Get-Trailing-CRLF

            buffer = new byte[1];

            while (true)
            {
                bytesRead = await Data.ReadAsync(buffer, token).ConfigureAwait(false);
                if (bytesRead > 0)
                {
                    if (buffer[0] == 10)
                        break;
                }
            }

            #endregion

            return chunk;
        }

        /// <summary>
        /// Determine if a header exists.
        /// </summary>
        /// <param name="key">Header key.</param>
        /// <returns>True if exists.</returns>
        public override bool HeaderExists(string key)
        {
            return string.IsNullOrEmpty(key)
                ? throw new ArgumentNullException(nameof(key))
                : Headers != null && Headers.AllKeys.Any(k => k.ToLower().Equals(key.ToLower()));
        }

        /// <summary>
        /// Determine if a querystring entry exists.
        /// </summary>
        /// <param name="key">Querystring key.</param>
        /// <returns>True if exists.</returns>
        public override bool QuerystringExists(string key)
        {
            return string.IsNullOrEmpty(key)
                ? throw new ArgumentNullException(nameof(key))
                : Query != null
                    && Query.Elements != null
                    && Query.Elements.AllKeys.Any(k => k.ToLower().Equals(key.ToLower()));
        }

        /// <summary>
        /// Retrieve a header (or querystring) value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Value.</returns>
        public override string RetrieveHeaderValue(string key)
        {
            return string.IsNullOrEmpty(key) ? throw new ArgumentNullException(nameof(key))
                : Headers != null ? Headers.Get(key)
                : null;
        }

        /// <summary>
        /// Retrieve a querystring value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Value.</returns>
        public override string RetrieveQueryValue(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (Query != null && Query.Elements != null)
            {
                var val = Query.Elements.Get(key);
                if (!string.IsNullOrEmpty(val))
                    val = WebUtility.UrlDecode(val);

                return val;
            }

            return null;
        }

        #endregion

        #region Private-Methods

        private static byte[] ReadStreamFully(Stream input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (!input.CanRead)
                throw new InvalidOperationException("Input stream is not readable");

            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;

                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);

                return ms.ToArray();
            }
        }

        #endregion
    }
}
